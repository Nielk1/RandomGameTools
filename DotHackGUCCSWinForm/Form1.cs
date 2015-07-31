using DotHackGUCCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DotHackGUCCSWinForm
{
    public partial class Form1 : Form
    {
        private DotHackGU2CCSFFile file;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    treeView1.Nodes.Clear();

                    using (FileStream dat = File.Open(openFileDialog1.FileName, FileMode.Open))
                    {
                        file = new DotHackGU2CCSFFile(dat);

                        //treeView1
                        file.BlockChain.ForEach(dr =>
                        {
                            treeView1.Nodes.Add(GenerateNodeTree(dr));
                        });
                    }
                }
            }
        }

        private TreeNode GenerateNodeTree(CCSBlock block)
        {
            string BlockName = block.GetNodeName(file.FileNamesBlock);
            TreeNode newNode = new TreeNode(block.ToString() + (BlockName != null && BlockName.Length > 0 ? " - " + BlockName : string.Empty));
            newNode.Tag = block;

            if (block.BlockChain != null)
                block.BlockChain.ForEach(dr =>
                {
                    newNode.Nodes.Add(GenerateNodeTree(dr));
                });

            return newNode;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pnlDetails.Controls.Clear();

            CCSBlock block = (CCSBlock)e.Node.Tag;
            switch (block.BlockType)
            {
                case CCSBlockType.Texture:
                    {
                        CCSTextureBlock tBlock = (CCSTextureBlock)block;

                        Panel centerPanel = new Panel();
                        centerPanel.AutoScroll = true;
                        pnlDetails.Controls.Add(centerPanel);
                        centerPanel.Dock = DockStyle.Fill;

                        PictureBox pBox = new PictureBox();
                        centerPanel.Controls.Add(pBox);

                        Color[] palletData = null;

                        foreach (CCSBlock palletBlock in file.BlockChain)
                        {
                            if (palletBlock.BlockType == CCSBlockType.Pallet)
                            {
                                CCSPalletBlock palletBlock2 = ((CCSPalletBlock)palletBlock);
                                if (palletBlock2.BakedFileIndex == tBlock.PalletFileIndex)
                                {
                                    palletData = palletBlock2.Colors;
                                    break;
                                }
                            }
                        }

                        Image blockImage = tBlock.GetImage(palletData);
                        pBox.Image = blockImage;
                        pBox.Size = blockImage.Size;
                    }
                    break;
                case CCSBlockType.Pallet:
                    {
                        CCSPalletBlock tBlock = (CCSPalletBlock)block;

                        EnhancedPicture pBox = new EnhancedPicture();

                        pnlDetails.Controls.Add(pBox);
                        pBox.Dock = DockStyle.Fill;

                        Image blockImage = tBlock.GetPalletImage();

                        pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pBox.Interpolation = InterpolationMode.NearestNeighbor;

                        pBox.Image = blockImage;
                        //pBox.Size = blockImage.Size;
                    }
                    break;
                case CCSBlockType.FileNames:
                    {
                        CCSFileNamesBlock tBlock = (CCSFileNamesBlock)block;

                        SplitContainer tmpPanel = new SplitContainer();

                        ListBox lstFileNames = new ListBox();
                        ListBox lstNodeNames = new ListBox();

                        Label lblFileNames = new Label();
                        Label lblNodeNames = new Label();

                        lblFileNames.Text = "File Names";
                        lblNodeNames.Text = "Node Names";

                        lblFileNames.TextAlign = ContentAlignment.MiddleCenter;
                        lblNodeNames.TextAlign = ContentAlignment.MiddleCenter;

                        tmpPanel.Panel1.Controls.Add(lstFileNames);
                        tmpPanel.Panel2.Controls.Add(lstNodeNames);

                        tmpPanel.Panel1.Controls.Add(lblFileNames);
                        tmpPanel.Panel2.Controls.Add(lblNodeNames);

                        tmpPanel.SplitterDistance = tmpPanel.Width / 2;
                        tmpPanel.IsSplitterFixed = true;

                        lblFileNames.Dock = DockStyle.Top;
                        lblNodeNames.Dock = DockStyle.Top;
                        lstFileNames.Dock = DockStyle.Fill;
                        lstNodeNames.Dock = DockStyle.Fill;

                        pnlDetails.Controls.Add(tmpPanel);

                        tmpPanel.Dock = DockStyle.Fill;

                        tBlock.BaseFiles.ToList().ForEach(dr =>
                        {
                            lstFileNames.Items.Add(dr);
                        });

                        tBlock.BakedFiles.ToList().ForEach(dr =>
                        {
                            lstNodeNames.Items.Add(dr);
                        });

                        lstNodeNames.SelectedIndexChanged += new EventHandler((sender2, e2) =>
                        {
                            if (lstNodeNames.SelectedItem != null)
                            {
                                lstFileNames.SelectedIndex = ((CCSFileNamesBlock.BakedFile)(lstNodeNames.SelectedItem)).Value;
                            }
                            else
                            {
                                lstFileNames.SelectedIndex = -1;
                            }
                        });

                        //if (tBlock.ExtraData.Count > 0)
                        //{
                        //    TextBox rawBox = new TextBox()
                        //    {
                        //        Multiline = true,
                        //        ReadOnly = true,
                        //        Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                        //        ScrollBars = ScrollBars.Vertical,
                        //        WordWrap = true,
                        //        Height = 200,
                        //        Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                        //    };
                        //    pnlDetails.Controls.Add(rawBox);
                        //    rawBox.Dock = DockStyle.Bottom;
                        //}
                    }
                    break;
                case CCSBlockType.FileHeader:
                    {
                        CCSFileHeaderBlock tBlock = (CCSFileHeaderBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        innerPanel.Controls.Add(new Label() { Text = "FileName:" }, 0, 0);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.Filename }, 1, 0);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown1:" }, 0, 1);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown1) }, 1, 1);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown2:" }, 0, 2);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown2) }, 1, 2);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown3:" }, 0, 3);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown3) }, 1, 3);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown4:" }, 0, 4);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown4) }, 1, 4);

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.Hierarchy:
                    {
                        CCSHierarchyBlock tBlock = (CCSHierarchyBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        innerPanel.Controls.Add(new Label() { Text = "BakedNodeIndex:" }, 0, 0);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex].Name) }, 1, 0);

                        innerPanel.Controls.Add(new Label() { Text = "ParentNodeIndex:" }, 0, 1);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.ParentBakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.ParentBakedFileIndex].Name) }, 1, 1);

                        innerPanel.Controls.Add(new Label() { Text = "BakedNodeIndex2:" }, 0, 2);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex2, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex2].Name) }, 1, 2);

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.Material:
                    {
                        CCSMaterialBlock tBlock = (CCSMaterialBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        innerPanel.Controls.Add(new Label() { Text = "BakedNodeIndex:" }, 0, 0);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex].Name) }, 1, 0);

                        innerPanel.Controls.Add(new Label() { Text = "TextureBakedNodeIndex:" }, 0, 1);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.TextureBakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.TextureBakedFileIndex].Name) }, 1, 1);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown1:" }, 0, 2);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.Unknown1.ToString("0.0") }, 1, 2);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown2:" }, 0, 3);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown2) }, 1, 3);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown3:" }, 0, 4);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown3) }, 1, 4);

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.Object:
                    {
                        CCSObjectBlock tBlock = (CCSObjectBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, 0);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.OBJBakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.OBJBakedFileIndex].Name) }, 1, 0);

                        innerPanel.Controls.Add(new Label() { Text = "Object2BakedIndex:" }, 0, 1);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.OBJ2BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.OBJ2BakedFileIndex].Name) }, 1, 1);

                        innerPanel.Controls.Add(new Label() { Text = "MDLBakedIndex:" }, 0, 2);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.MDLBakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.MDLBakedFileIndex].Name) }, 1, 2);

                        int x = 0;
                        for (; x < tBlock.RawData.Count; x++)
                        {
                            innerPanel.Controls.Add(new Label() { Text = string.Format("Unknown{0}:", x + 1) }, 0, x + 3);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.RawData[x]) }, 1, x + 3);
                            //innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.RawData[x], file.FileNamesBlock.BakedFiles[tBlock.RawData[x]].Name) }, 1, x + 3);
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.Mesh:
                    {
                        CCSMeshBlock tBlock = (CCSMeshBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.SelfIndex, file.FileNamesBlock.BakedFiles[tBlock.SelfIndex].Name) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown1:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown1) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown2A:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,4:X4}", tBlock.Unknown2A) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "SubmeshCount:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,4:X4}", tBlock.SubmeshCount) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown3:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown3) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown4:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown4) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown5:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown5) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "UnknownFloat:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.UnknownFloat.ToString("0.0") }, 1, row++);

                        /*if (tBlock.Unknown1 == 0 || (tBlock.RawData != null && tBlock.RawData.Length > 0))
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.RawData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, row++);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }
                        else*/
                        for (int meshNum = 0; meshNum < tBlock.Meshes.Count; meshNum++)
                        {
                            CCSMeshBlock.Mesh meshData = tBlock.Meshes.ElementAt(meshNum);

                            Label startModel = new Label() { Text = string.Format("--------------[Mesh{0,2}]--------------", meshNum.ToString().PadLeft(2, '0')) };
                            innerPanel.Controls.Add(startModel, 0, row++);
                            innerPanel.SetColumnSpan(startModel, 2);
                            startModel.Dock = DockStyle.Fill;

                            {
                                Button btnPreview = new Button() { Text = "Preview" };
                                innerPanel.Controls.Add(btnPreview, 0, row++);
                                innerPanel.SetColumnSpan(btnPreview, 2);
                                btnPreview.Dock = DockStyle.Fill;

                                CCSBlock _matBlock = file.BlockChain.Where(dr => dr.BlockType == CCSBlockType.Material && ((CCSMaterialBlock)dr).BakedFileIndex == meshData.MaterialIndex).FirstOrDefault();
                                CCSMaterialBlock matBlock = _matBlock != null ? (CCSMaterialBlock)_matBlock : null;
                                CCSBlock _textBlock = matBlock != null ? file.BlockChain.Where(dr => dr.BlockType == CCSBlockType.Texture && ((CCSTextureBlock)dr).BakedFileIndex == matBlock.TextureBakedFileIndex).FirstOrDefault() : null;
                                CCSTextureBlock textBlock = _textBlock != null ? (CCSTextureBlock)_textBlock : null;
                                CCSBlock _palletBlock = textBlock != null ? file.BlockChain.Where(dr => dr.BlockType == CCSBlockType.Pallet && ((CCSPalletBlock)dr).BakedFileIndex == textBlock.PalletFileIndex).FirstOrDefault() : null;
                                CCSPalletBlock palletBlock = _palletBlock != null ? (CCSPalletBlock)_palletBlock : null;

                                btnPreview.Click += delegate {
                                    ModelPreview previewForm = new ModelPreview(meshData, matBlock, textBlock, palletBlock);
                                    previewForm.Show();
                                };
                            }

                            innerPanel.Controls.Add(new Label() { Text = "UnknownA1:" }, 0, row);
                            if (file.FileNamesBlock.BakedFiles.Length > meshData.UnknownA1)
                            {
                                innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", meshData.UnknownA1, file.FileNamesBlock.BakedFiles[meshData.UnknownA1].Name) }, 1, row++);
                            }
                            else
                            {
                                innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,2:X2}", meshData.UnknownA1) }, 1, row++);
                            }

                            innerPanel.Controls.Add(new Label() { Text = "MaterialBakedIndex:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", meshData.MaterialIndex, file.FileNamesBlock.BakedFiles[meshData.MaterialIndex].Name) }, 1, row++);

                            innerPanel.Controls.Add(new Label() { Text = "VertCount:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", meshData.VertCount) }, 1, row++);


                            innerPanel.Controls.Add(new Label() { Text = "Vertex List:" }, 0, row++);

                            ListBox VertexList = new ListBox();
                            innerPanel.Controls.Add(VertexList, 0, row++);
                            innerPanel.SetColumnSpan(VertexList, 2);
                            VertexList.Dock = DockStyle.Fill;
                            VertexList.BeginUpdate();
                            meshData.Verts.ForEach(dr => VertexList.Items.Add(dr));
                            VertexList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "Normal List:" }, 0, row++);

                            ListBox NormList = new ListBox();
                            innerPanel.Controls.Add(NormList, 0, row++);
                            innerPanel.SetColumnSpan(NormList, 2);
                            NormList.Dock = DockStyle.Fill;
                            NormList.BeginUpdate();
                            meshData.Norms.ForEach(dr => NormList.Items.Add(dr));
                            NormList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "RGB List:" }, 0, row++);

                            ListBox RGBList = new ListBox();
                            innerPanel.Controls.Add(RGBList, 0, row++);
                            innerPanel.SetColumnSpan(RGBList, 2);
                            RGBList.Dock = DockStyle.Fill;
                            RGBList.BeginUpdate();
                            meshData.RGBAs.ForEach(dr => RGBList.Items.Add(dr));
                            RGBList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "UV List:" }, 0, row++);

                            ListBox UVList = new ListBox();
                            innerPanel.Controls.Add(UVList, 0, row++);
                            innerPanel.SetColumnSpan(UVList, 2);
                            UVList.Dock = DockStyle.Fill;
                            UVList.BeginUpdate();
                            meshData.UVs.ForEach(dr => UVList.Items.Add(dr));
                            UVList.EndUpdate();
                        }
                        if (tBlock.Meshes.Count > 0)
                        {
                            Label endModels = new Label() { Text = "------------------------------------------" };
                            innerPanel.Controls.Add(endModels, 0, row++);
                            innerPanel.SetColumnSpan(endModels, 2);
                            endModels.Dock = DockStyle.Fill;
                        }

                        for (int meshNum = 0; meshNum < tBlock.shadowMesh.Count; meshNum++)
                        {
                            CCSMeshBlock.ShadowMesh meshData = tBlock.shadowMesh.ElementAt(meshNum);

                            Label startModel = new Label() { Text = string.Format("--------------[ShadowMesh{0}]--------------", meshNum.ToString().PadLeft(2,'0')) };
                            innerPanel.Controls.Add(startModel, 0, row++);
                            innerPanel.SetColumnSpan(startModel, 2);
                            startModel.Dock = DockStyle.Fill;

                            {
                                Button btnPreview = new Button() { Text = "Preview" };
                                innerPanel.Controls.Add(btnPreview, 0, row++);
                                innerPanel.SetColumnSpan(btnPreview, 2);
                                btnPreview.Dock = DockStyle.Fill;

                                //CCSBlock _matBlock = file.BlockChain.Where(dr => dr.BlockType == CCSBlockType.Material && ((CCSMaterialBlock)dr).BakedFileIndex == meshData.MaterialIndex).First();
                                //CCSMaterialBlock matBlock = _matBlock != null ? (CCSMaterialBlock)_matBlock : null;
                                //CCSBlock _textBlock = matBlock != null ? file.BlockChain.Where(dr => dr.BlockType == CCSBlockType.Texture && ((CCSTextureBlock)dr).BakedFileIndex == matBlock.TextureBakedFileIndex).First() : null;
                                //CCSTextureBlock textBlock = _textBlock != null ? (CCSTextureBlock)_textBlock : null;
                                //CCSBlock _palletBlock = textBlock != null ? file.BlockChain.Where(dr => dr.BlockType == CCSBlockType.Pallet && ((CCSPalletBlock)dr).BakedFileIndex == textBlock.PalletFileIndex).First() : null;
                                //CCSPalletBlock palletBlock = _palletBlock != null ? (CCSPalletBlock)_palletBlock : null;

                                btnPreview.Click += delegate {
                                    ModelPreview previewForm = new ModelPreview(meshData);//, matBlock, textBlock, palletBlock);
                                    previewForm.Show();
                                };
                            }

                            innerPanel.Controls.Add(new Label() { Text = "VertCount:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", meshData.VertexCount) }, 1, row++);

                            innerPanel.Controls.Add(new Label() { Text = "Vertex List:" }, 0, row++);

                            ListBox VertexList = new ListBox();
                            innerPanel.Controls.Add(VertexList, 0, row++);
                            innerPanel.SetColumnSpan(VertexList, 2);
                            VertexList.Dock = DockStyle.Fill;
                            VertexList.BeginUpdate();
                            meshData.Verts.ForEach(dr => VertexList.Items.Add(dr));
                            VertexList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "FaceCount:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", meshData.FaceCount) }, 1, row++);

                            innerPanel.Controls.Add(new Label() { Text = "Face List:" }, 0, row++);

                            ListBox FaceList = new ListBox();
                            innerPanel.Controls.Add(FaceList, 0, row++);
                            innerPanel.SetColumnSpan(FaceList, 2);
                            FaceList.Dock = DockStyle.Fill;
                            FaceList.BeginUpdate();
                            meshData.Faces.ForEach(dr => FaceList.Items.Add(dr));
                            FaceList.EndUpdate();
                        }
                        if (tBlock.shadowMesh.Count > 0)
                        {
                            Label endModels = new Label() { Text = "------------------------------------------" };
                            innerPanel.Controls.Add(endModels, 0, row++);
                            innerPanel.SetColumnSpan(endModels, 2);
                            endModels.Dock = DockStyle.Fill;
                        }

                        if (tBlock.ExtraData.Count > 0)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, row++);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.CCCCFF01:
                    {
                        CCSCCCCFF01Block tBlock = (CCSCCCCFF01Block)block;

                        if (tBlock.ExtraData.Count > 0)
                        {
                            //innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            //innerPanel.Controls.Add(rawBox, 0, row++);
                            //innerPanel.SetColumnSpan(rawBox, 2);

                            pnlDetails.Controls.Add(rawBox);

                            rawBox.Dock = DockStyle.Fill;
                        }

                        /*TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "EndMarker:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0,8:X8}", tBlock.EndMarker) }, 1, row++);

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);*/
                    }
                    break;
                case CCSBlockType.CCCC0003:
                    {
                        CCSCCCC0003Block tBlock = (CCSCCCC0003Block)block;

                        if (tBlock.ExtraData.Count > 0)
                        {
                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            pnlDetails.Controls.Add(rawBox);
                            rawBox.Dock = DockStyle.Fill;
                        }
                    }
                    break;
                case CCSBlockType.CCCC0005:
                    {
                        CCSCCCC0005Block tBlock = (CCSCCCC0005Block)block;

                        if (tBlock.ExtraData.Count > 0)
                        {
                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            pnlDetails.Controls.Add(rawBox);
                            rawBox.Dock = DockStyle.Fill;
                        }
                    }
                    break;
                case CCSBlockType.CCCC1F00:
                    {
                        CCSCCCC1F00Block tBlock = (CCSCCCC1F00Block)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.SelfIndex, file.FileNamesBlock.BakedFiles[tBlock.SelfIndex].Name) }, 1, row++);

                        if (tBlock.ExtraData.Count > 0)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, row++);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.CCCC0102:
                    {
                        CCSCCCC0102Block tBlock = (CCSCCCC0102Block)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "SelfIndex:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock, file.FileNamesBlock.BakedFiles[tBlock.SelfIndex].Name) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Marker Switches:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = Convert.ToString(tBlock.Marker, 2).PadLeft(32,'0') }, 1, row++);

                        if(tBlock.DataSet0 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data0:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.DataSet0.ToString() }, 1, row++);
                        }

                        if (tBlock.DataSet1 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data1 Count:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.DataSet1.Count) }, 1, row++);


                            innerPanel.Controls.Add(new Label() { Text = "Data1 List:" }, 0, row++);

                            TextBox ListOfItems = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join("\r\n", tBlock.DataSet1.Select(dr => dr).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(ListOfItems, 0, row++);
                            innerPanel.SetColumnSpan(ListOfItems, 2);
                            ListOfItems.Dock = DockStyle.Fill;
                            //ListOfItems.BeginUpdate();
                            //tBlock.DataSet1.ForEach(dr => ListOfItems.Items.Add(dr));
                            //ListOfItems.EndUpdate();
                        }

                        if (tBlock.DataSet3 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data3:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.DataSet3.ToString() }, 1, row++);
                        }

                        if (tBlock.DataSet4 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data4 Count:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.DataSet4.Count) }, 1, row++);


                            innerPanel.Controls.Add(new Label() { Text = "Data4 List:" }, 0, row++);

                            TextBox ListOfItems = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join("\r\n", tBlock.DataSet4.Select(dr => dr).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(ListOfItems, 0, row++);
                            innerPanel.SetColumnSpan(ListOfItems, 2);
                            ListOfItems.Dock = DockStyle.Fill;
                            //ListOfItems.BeginUpdate();
                            //tBlock.DataSet4.ForEach(dr => ListOfItems.Items.Add(dr));
                            //ListOfItems.EndUpdate();
                        }

                        if (tBlock.DataSet5 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data5 Count:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.DataSet5.Count) }, 1, row++);


                            innerPanel.Controls.Add(new Label() { Text = "Data5 List:" }, 0, row++);

                            TextBox ListOfItems = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join("\r\n", tBlock.DataSet5.Select(dr => dr).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(ListOfItems, 0, row++);
                            innerPanel.SetColumnSpan(ListOfItems, 2);
                            ListOfItems.Dock = DockStyle.Fill;
                            //ListOfItems.BeginUpdate();
                            //tBlock.DataSet5.ForEach(dr => ListOfItems.Items.Add(dr));
                            //ListOfItems.EndUpdate();
                        }

                        if (tBlock.DataSet6 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data6:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.DataSet6.ToString() }, 1, row++);
                        }

                        if (tBlock.DataSet7 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data7 Count:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.DataSet7.Count) }, 1, row++);


                            innerPanel.Controls.Add(new Label() { Text = "Data7 List:" }, 0, row++);

                            TextBox ListOfItems = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join("\r\n", tBlock.DataSet7.Select(dr => dr).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(ListOfItems, 0, row++);
                            innerPanel.SetColumnSpan(ListOfItems, 2);
                            ListOfItems.Dock = DockStyle.Fill;
                            //ListOfItems.BeginUpdate();
                            //tBlock.DataSet7.ForEach(dr => ListOfItems.Items.Add(dr));
                            //ListOfItems.EndUpdate();
                        }

                        if (tBlock.DataSet9 != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Data9:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.DataSet9.HasValue ? string.Format("{0,8:X8}", tBlock.DataSet9) : string.Empty }, 1, row++);
                        }

                        if (tBlock.DataSetA != null)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "DataA Count:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.DataSetA.Count) }, 1, row++);


                            innerPanel.Controls.Add(new Label() { Text = "DataA List:" }, 0, row++);

                            TextBox ListOfItems = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join("\r\n", tBlock.DataSetA.Select(dr => dr).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(ListOfItems, 0, row++);
                            innerPanel.SetColumnSpan(ListOfItems, 2);
                            ListOfItems.Dock = DockStyle.Fill;
                            //ListOfItems.BeginUpdate();
                            //tBlock.DataSetA.ForEach(dr => ListOfItems.Items.Add(dr));
                            //ListOfItems.EndUpdate();
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        //innerPanel.Dock = DockStyle.Fill;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.CCCC2000:
                    {
                        CCSCCCC2000Block tBlock = (CCSCCCC2000Block)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex].Name) }, 1, row++);

                        if (tBlock.ExtraData.Count > 0)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, row++);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;

                case CCSBlockType.Composit:
                    {
                        CCSCompositBlock tBlock = (CCSCompositBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex].Name) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Count Subnodes:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.CountSubnodes) }, 1, row++);

                        for (int x = 0; x < tBlock.SubnodeBakedFileIndexs.Count; x++)
                        {
                            innerPanel.Controls.Add(new Label() { Text = @"Baked Subnode " + x + @":" }, 0, row);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.SubnodeBakedFileIndexs[x], file.FileNamesBlock.BakedFiles[tBlock.SubnodeBakedFileIndexs[x]].Name) }, 1, row++);
                        }

                        for (int x = 0; x < tBlock.SubnodeDataItems.Count; x++)
                        {
                            Label startModel = new Label() { Text = string.Format("--------------[Matrix{0,2}]--------------", x.ToString().PadLeft(2, '0')) };
                            innerPanel.Controls.Add(startModel, 0, row++);
                            innerPanel.SetColumnSpan(startModel, 2);
                            startModel.Dock = DockStyle.Fill;

                            innerPanel.Controls.Add(new Label() { Text = "Translate:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox()
                            {
                                ReadOnly = true,
                                Width = 200,
                                Text = tBlock.SubnodeDataItems[x].tx.ToString("0.0") + "\t" + tBlock.SubnodeDataItems[x].ty.ToString("0.0") + "\t" + tBlock.SubnodeDataItems[x].tz.ToString("0.0")
                            }, 1, row++);

                            innerPanel.Controls.Add(new Label() { Text = "Rotation:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox()
                            {
                                ReadOnly = true,
                                Width = 200,
                                Text = tBlock.SubnodeDataItems[x].rx.ToString("0.0") + "\t" + tBlock.SubnodeDataItems[x].ry.ToString("0.0") + "\t" + tBlock.SubnodeDataItems[x].rz.ToString("0.0")
                            }, 1, row++);

                            innerPanel.Controls.Add(new Label() { Text = "Scale:" }, 0, row);
                            innerPanel.Controls.Add(new TextBox()
                            {
                                ReadOnly = true,
                                Width = 200,
                                Text = tBlock.SubnodeDataItems[x].sx.ToString("0.0") + "\t" + tBlock.SubnodeDataItems[x].sy.ToString("0.0") + "\t" + tBlock.SubnodeDataItems[x].sz.ToString("0.0")
                            }, 1, row++);
                        }
                        if (tBlock.SubnodeDataItems.Count > 0)
                        {
                            Label endModels = new Label() { Text = "------------------------------------------" };
                            innerPanel.Controls.Add(endModels, 0, row++);
                            innerPanel.SetColumnSpan(endModels, 2);
                            endModels.Dock = DockStyle.Fill;
                        }

                        if (tBlock.ExtraData.Count > 0)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, row++);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.CCCC0202:
                    {
                        CCSCCCC0202Block tBlock = (CCSCCCC0202Block)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        //innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, row);
                        //innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex].Name) }, 1, row++);
                        
                        //innerPanel.Controls.Add(new Label() { Text = "Count Subnodes:" }, 0, row);
                        //innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.CountSubnodes) }, 1, row++);

                        if (tBlock.ExtraData.Count > 0)
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);

                            TextBox rawBox = new TextBox()
                            {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, row++);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
                case CCSBlockType.Animation:
                    {
                        CCSAnimationBlock tBlock = (CCSAnimationBlock)block;

                        TableLayoutPanel innerPanel = new TableLayoutPanel();

                        int row = 0;

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.BakedFileIndex, file.FileNamesBlock.BakedFiles[tBlock.BakedFileIndex].Name) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown1:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown1) }, 1, row++);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown2:" }, 0, row);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown2) }, 1, row++);

                        //if (tBlock.ExtraData.Count > 0)
                        //{
                        //    innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, row++);
                        //
                        //    TextBox rawBox = new TextBox()
                        //    {
                        //        Multiline = true,
                        //        ReadOnly = true,
                        //        Text = string.Join(" ", tBlock.ExtraData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                        //        ScrollBars = ScrollBars.Vertical,
                        //        WordWrap = true,
                        //        Height = 200,
                        //        Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                        //    };
                        //    innerPanel.Controls.Add(rawBox, 0, row++);
                        //    innerPanel.SetColumnSpan(rawBox, 2);
                        //    rawBox.Dock = DockStyle.Fill;
                        //}

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
            }
        }
    }
}
