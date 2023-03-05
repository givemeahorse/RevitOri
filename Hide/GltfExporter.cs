using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RevitAsset = Autodesk.Revit.DB.Visual.Asset;

namespace Hide
{
    public class GltfExporter : IExportContext
    {

        #region field
        private string outputPath;

        private int levelOfDetail;
        private List<int> filter;

        private Document doc;

        private Stack<Document> docs = new Stack<Document>();
        private Stack<Transform> transforms = new Stack<Transform>();

        private ModelRoot gltfRootModel;
        //private MeshBuilder<VertexPosition> meshBuilder;

        private MeshBuilder<VertexPosition, VertexTexture1> meshBuilder;
        private string textureFolder;

        private Dictionary<string, MaterialBuilder> materialMapping = new Dictionary<string, MaterialBuilder>();
        private MaterialBuilder currentMaterial;
        #endregion

        public GltfExporter(Document doc, ExportConfig config)
        {
            this.doc = doc;
            outputPath = config.OutputPath;
            levelOfDetail = config.LevelOfDetail;
            filter = config.Filter;
            Initial();
        }


        private void Initial()
        {
            docs.Push(doc);
            transforms.Push(Transform.Identity);
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey libraryPath = hklm.OpenSubKey("SOFTWARE\\WOW6432Node\\Autodesk\\ADSKTextureLibrary\\1");
            textureFolder = libraryPath.GetValue("LibraryPaths").ToString();
            libraryPath.Close();
            hklm.Close();
        }

        public void Finish()
        {
            if (this.meshBuilder != null && this.meshBuilder.Primitives.Count > 0)
            {
                SharpGLTF.Schema2.Mesh meshInstance = this.gltfRootModel.CreateMeshes(new IMeshBuilder<MaterialBuilder>[]
                {
                    this.meshBuilder
                }).First<SharpGLTF.Schema2.Mesh>();
                this.gltfRootModel.UseScene("Default").CreateNode(null).WithMesh(meshInstance);
            }
            if (gltfRootModel != null)
                gltfRootModel.SaveGLTF(outputPath);
        }

        public bool IsCanceled()
        {
            return false;
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            if (!filter.Contains(elementId.IntegerValue))
                return RenderNodeAction.Skip;
            var element = docs.Peek().GetElement(elementId);
            //meshBuilder = new MeshBuilder<VertexPosition>(element.Name);
            meshBuilder = new MeshBuilder<VertexPosition, VertexTexture1>();
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            var element = docs.Peek().GetElement(elementId);
            if (meshBuilder != null && meshBuilder.Primitives.Count > 0)
            {
                SharpGLTF.Schema2.Mesh meshInstance = this.gltfRootModel.CreateMeshes(new IMeshBuilder<MaterialBuilder>[]
                {
                    this.meshBuilder
                }).First<SharpGLTF.Schema2.Mesh>();
                var node = this.gltfRootModel.UseScene("Default").CreateNode(element.Name).WithMesh(meshInstance);
                node.Extras = SharpGLTF.IO.JsonContent.Serialize(elementId);
            }
            meshBuilder = null;
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {

        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            var transform = node.GetTransform();
            transform = transforms.Peek().Multiply(transform);
            transforms.Push(transform);
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            transforms.Pop();
        }

        public void OnLight(LightNode node)
        {

        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            docs.Push(node.GetDocument());
            transforms.Push(transforms.Peek().Multiply(node.GetTransform()));
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            docs.Pop();
            transforms.Pop();
        }

        public void OnMaterial(MaterialNode node)
        {
            currentMaterial = GetMaterial(node);
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            try
            {
                var primitive = meshBuilder.UsePrimitive(currentMaterial);
                var points = node.GetPoints();
                var normalsInGltf = new List<VertexPositionNormal>();
                var pointsInGltf = new List<VertexPosition>();
                var textureInGltf = new List<VertexTexture1>();
                var uvInGltf = new List<UV>();
                foreach (var uv in node.GetUVs())
                {
                    uvInGltf.Add(uv);
                }
                var transform = transforms.Peek();
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = transform.OfPoint(points[i]);
                    pointsInGltf.Add(new VertexPosition(
                        (float)points[i].X,
                         (float)points[i].Y,
                          (float)points[i].Z));
                }


                foreach (var facet in node.GetFacets())
                {
                    var uv1 = new Vector2((float)uvInGltf[facet.V1].U, (float)uvInGltf[facet.V1].V);
                    var uv2 = new Vector2((float)uvInGltf[facet.V2].U, (float)uvInGltf[facet.V2].V);
                    var uv3 = new Vector2((float)uvInGltf[facet.V3].U, (float)uvInGltf[facet.V3].V);

                    primitive.AddTriangle(
                       new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(pointsInGltf[facet.V1], new VertexTexture1(uv1)),
                       new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(pointsInGltf[facet.V2], new VertexTexture1(uv2)),
                       new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(pointsInGltf[facet.V3], new VertexTexture1(uv3)));
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("BIMBOX", e.Message);
            }

        }


        public void OnPolymesh1(PolymeshTopology node)
        {
            try
            {
                var primitive = meshBuilder.UsePrimitive(currentMaterial);
                var points = node.GetPoints();
                var pointsInGltf = new List<VertexPosition>();
                var transform = transforms.Peek();
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = transform.OfPoint(points[i]);
                    pointsInGltf.Add(new VertexPosition(
                        (float)points[i].X,
                         (float)points[i].Y,
                          (float)points[i].Z));
                }
                foreach (var facet in node.GetFacets())
                {
                    primitive.AddTriangle(
                        pointsInGltf[facet.V1],
                        pointsInGltf[facet.V2],
                        pointsInGltf[facet.V3]);
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("BIMBOX", e.Message);
            }
        }

        public void OnRPC(RPCNode node)
        {

        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            node.LevelOfDetail = levelOfDetail;
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {

        }

        public bool Start()
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                currentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                gltfRootModel = ModelRoot.CreateModel();
                gltfRootModel.Asset.Copyright = "BIMBOX";

                return true;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return false;
            }

        }

        private MaterialBuilder GetMaterial(MaterialNode node)
        {
            if (node.Color.IsValid)
            {
                if (node.MaterialId != ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.Material material = docs.Peek().GetElement(node.MaterialId) as Autodesk.Revit.DB.Material;
                    if (material != null)
                    {
                        var uri = GetTexturePath(node, out Vector2 scale);
                        if (File.Exists(uri))
                        {
                            var image = new MemoryImage(uri);
                            var builder = BuildMaterial(material, image, scale);

                            return builder;
                        }
                    }
                }
                return BuildMaterial(node);
            }

            return new MaterialBuilder().WithDoubleSide(true).WithMetallicRoughnessShader().WithChannelParam("BaseColor", new Vector4(1f, 0f, 0f, 1f));
        }

        private string GetTexturePath(MaterialNode node, out Vector2 scale)
        {
            RevitAsset asset = node.HasOverriddenAppearance ? node.GetAppearanceOverride() : node.GetAppearance();
            scale = GetTextureScale(asset);
            var ap = FindTextureAsset(asset);
            if (ap != null)
            {

                var textureFile = (ap.FindByName("unifiedbitmap_Bitmap") as AssetPropertyString).Value.Split('|')[0];
                return Path.Combine(textureFolder, textureFile.Replace("/", "\\"));
            }
            return string.Empty;
        }

        private Vector2 GetTextureScale(RevitAsset asset)
        {
            var scale = new Vector2(1, 1);
            for (int j = 0; j < asset.Size; j++)
            {
                IList<AssetProperty> allConnectedProperties = asset[j].GetAllConnectedProperties();
                if (allConnectedProperties != null && allConnectedProperties.Count != 0)
                {
                    RevitAsset asset2 = allConnectedProperties.First() as RevitAsset;
                    for (int k = 0; k < asset2.Size; k++)
                    {
                        AssetPropertyDistance assetPropertyDistance = asset2[k] as AssetPropertyDistance;
                        if (assetPropertyDistance != null)
                        {
                            //double num = GetUnitTypeScale(assetPropertyDistance.DisplayUnitType);
                            double num = 1;
                            if (assetPropertyDistance.Name == "texture_RealWorldScaleX" || assetPropertyDistance.Name == "unifiedbitmap_RealWorldScaleX")
                            {
                                scale.X = (float)(1.0 / UnitUtils.ConvertToInternalUnits(assetPropertyDistance.Value, assetPropertyDistance.DisplayUnitType));
                            }
                            if (assetPropertyDistance.Name == "texture_RealWorldScaleY" || assetPropertyDistance.Name == "unifiedbitmap_RealWorldScaleY")
                            {
                                scale.Y = (float)(1.0 / UnitUtils.ConvertToInternalUnits(assetPropertyDistance.Value, assetPropertyDistance.DisplayUnitType));
                            }
                        }
                    }
                }
            }
            return scale;
        }

        private RevitAsset FindTextureAsset(AssetProperty ap)
        {
            if (ap == null) return null;
            if (ap.Type == AssetPropertyType.Asset)
            {
                var ra = ap as RevitAsset;
                if (!IsTextureAsset(ra))
                {
                    for (int i = 0; i < ra.Size; i++)
                    {
                        var asset = FindTextureAsset(ra[i]);
                        if (asset != null)
                        {
                            return asset;
                        }
                    }
                }
                else
                {
                    return ra;
                }
            }
            else
            {
                for (int j = 0; j < ap.NumberOfConnectedProperties; j++)
                {
                    var asset = FindTextureAsset(ap.GetConnectedProperty(j));
                    if (asset != null)
                    {
                        return asset;
                    }
                }
            }
            return null;
        }

        private bool IsTextureAsset(RevitAsset asset)
        {
            AssetProperty assetProprty = GetAssetProprty(asset, "assettype");
            if (assetProprty != null && (assetProprty as AssetPropertyString).Value == "texture")
            {
                return true;
            }
            return GetAssetProprty(asset, "unifiedbitmap_Bitmap") != null;
        }

        private AssetProperty GetAssetProprty(RevitAsset asset, string propertyName)
        {
            for (int i = 0; i < asset.Size; i++)
            {
                if (asset[i].Name == propertyName)
                {
                    return asset[i];
                }
            }
            return null;
        }


        private MaterialBuilder BuildMaterial(Autodesk.Revit.DB.Material material, MemoryImage image, Vector2 scale)
        {
            var matBuilder = new MaterialBuilder();
            var key = $"{material.Color.Red} {material.Color.Green} {material.Color.Blue} {material.Transparency}";
            if (materialMapping.ContainsKey(key))
                return materialMapping[key];
            if (material.Transparency != 0)
            {
                matBuilder = matBuilder.WithAlpha(SharpGLTF.Materials.AlphaMode.BLEND, 0.5f).WithChannelParam("BaseColor", new Vector4((float)material.Color.Red / 255f, (float)material.Color.Green / 255f, (float)material.Color.Blue / 255f, 1f - (float)material.Transparency / 255f)).WithChannelImage(KnownChannel.BaseColor, image);
                matBuilder.GetChannel(KnownChannel.BaseColor).Texture.WithTransform(Vector2.Zero, scale, 0);

            }
            else
            {
                matBuilder = matBuilder.WithDoubleSide(true).WithMetallicRoughnessShader().WithChannelParam("BaseColor", new Vector4((float)material.Color.Red / 255f, (float)material.Color.Green / 255f, (float)material.Color.Blue / 255f, 1f)).WithChannelImage(KnownChannel.BaseColor, image);

                matBuilder.GetChannel(KnownChannel.BaseColor).Texture.WithTransform(Vector2.Zero, scale, 0);

            }
            materialMapping[key] = matBuilder;
            return matBuilder;
        }


        private MaterialBuilder GetMaterial1(MaterialNode node)
        {
            if (node.Color.IsValid)
            {
                if (node.MaterialId != ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.Material material = this.docs.Peek().GetElement(node.MaterialId) as Autodesk.Revit.DB.Material;
                    if (material != null)
                    {
                        return BuildMaterial(material);
                    }
                }
                return BuildMaterial(node);
            }
            return new MaterialBuilder().WithDoubleSide(true).WithMetallicRoughnessShader().WithChannelParam("BaseColor", new Vector4(1f, 0f, 0f, 1f));
        }

        private MaterialBuilder BuildMaterial(MaterialNode node)
        {
            var key = $"{node.Color.Red} {node.Color.Green} {node.Color.Blue} {node.Transparency}";
            if (materialMapping.ContainsKey(key))
                return materialMapping[key];

            MaterialBuilder matBuilder = new MaterialBuilder();
            if (node.Transparency != 0d)
            {
                matBuilder = matBuilder.WithAlpha(SharpGLTF.Materials.AlphaMode.BLEND, 0.5f).WithChannelParam("BaseColor", new System.Numerics.Vector4((float)node.Color.Red / 255f, (float)node.Color.Green / 255f, (float)node.Color.Blue / 255f, 1f - (float)node.Transparency));
            }
            else
            {
                matBuilder = matBuilder.WithDoubleSide(true).WithMetallicRoughnessShader().WithChannelParam("BaseColor", new System.Numerics.Vector4((float)node.Color.Red / 255f, (float)node.Color.Green / 255f, (float)node.Color.Blue / 255f, 1f));
            }
            materialMapping[key] = matBuilder;
            return matBuilder;
        }

        private MaterialBuilder BuildMaterial(Autodesk.Revit.DB.Material material)
        {
            var matBuilder = new MaterialBuilder();
            var key = $"{material.Color.Red} {material.Color.Green} {material.Color.Blue} {material.Transparency}";
            if (materialMapping.ContainsKey(key))
                return materialMapping[key];
            if (material.Transparency != 0)
            {
                matBuilder = matBuilder.WithAlpha(SharpGLTF.Materials.AlphaMode.BLEND, 0.5f).WithChannelParam("BaseColor", new Vector4((float)material.Color.Red / 255f, (float)material.Color.Green / 255f, (float)material.Color.Blue / 255f, 1f - (float)material.Transparency / 255f));
            }
            else
            {
                matBuilder = matBuilder.WithDoubleSide(true).WithMetallicRoughnessShader().WithChannelParam("BaseColor", new Vector4((float)material.Color.Red / 255f, (float)material.Color.Green / 255f, (float)material.Color.Blue / 255f, 1f));
            }
            materialMapping[key] = matBuilder;
            return matBuilder;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + args.Name.Split(new char[]
            {
                ','
            })[0] + ".dll";
            if (File.Exists(file))
            {
                return Assembly.LoadFile(file);
            }
            return null;
        }
    }
}
