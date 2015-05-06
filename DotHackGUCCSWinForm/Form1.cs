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
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
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
            switch(block.BlockType)
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

                        foreach(CCSBlock palletBlock in file.BlockChain)
                        {
                            if(palletBlock.BlockType == CCSBlockType.Pallet)
                            {
                                CCSPalletBlock palletBlock2 = ((CCSPalletBlock)palletBlock);
                                if(palletBlock2.BakedFileIndex == tBlock.PalletFileIndex)
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
                            lstFileNames.SelectedIndex = ((CCSFileNamesBlock.BackedFile)(lstNodeNames.SelectedItem)).Value;
                        });
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
                case CCSBlockType.CCCC0100:
                    {
                        CCSCCCC0100Block tBlock = (CCSCCCC0100Block)block;

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

                        innerPanel.Controls.Add(new Label() { Text = "ObjectBakedIndex:" }, 0, 0);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.SelfIndex, file.FileNamesBlock.BakedFiles[tBlock.SelfIndex].Name) }, 1, 0);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown1:" }, 0, 1);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown1) }, 1, 1);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown2:" }, 0, 2);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown2) }, 1, 2);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown3:" }, 0, 3);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown3) }, 1, 3);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown4:" }, 0, 4);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown4) }, 1, 4);

                        innerPanel.Controls.Add(new Label() { Text = "Unknown5:" }, 0, 5);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,8:X8}", tBlock.Unknown5) }, 1, 5);

                        innerPanel.Controls.Add(new Label() { Text = "UnknownFloat:" }, 0, 6);
                        innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = tBlock.UnknownFloat.ToString("0.0") }, 1, 6);

                        if (tBlock.Unknown1 == 0 || (tBlock.RawData != null && tBlock.RawData.Length > 0))
                        {
                            innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, 7);

                            TextBox rawBox = new TextBox() {
                                Multiline = true,
                                ReadOnly = true,
                                Text = string.Join(" ", tBlock.RawData.Select(dr => string.Format("{0,2:X2}", dr)).ToArray()),
                                ScrollBars = ScrollBars.Vertical,
                                WordWrap = true,
                                Height = 200,
                                Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
                            };
                            innerPanel.Controls.Add(rawBox, 0, 8);
                            innerPanel.SetColumnSpan(rawBox, 2);
                            rawBox.Dock = DockStyle.Fill;
                        }
                        else
                        {
                            innerPanel.Controls.Add(new Label() { Text = "UnknownA1:" }, 0, 7);
                            if (file.FileNamesBlock.BakedFiles.Length > tBlock.UnknownA1)
                            {
                                innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.UnknownA1, file.FileNamesBlock.BakedFiles[tBlock.UnknownA1].Name) }, 1, 7);
                            }
                            else
                            {
                                innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("0x{0,2:X2}", tBlock.UnknownA1) }, 1, 7);
                            }

                            innerPanel.Controls.Add(new Label() { Text = "MaterialBakedIndex:" }, 0, 8);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("[{0}] {1}", tBlock.MaterialIndex, file.FileNamesBlock.BakedFiles[tBlock.MaterialIndex].Name) }, 1, 8);

                            innerPanel.Controls.Add(new Label() { Text = "VertCount:" }, 0, 9);
                            innerPanel.Controls.Add(new TextBox() { ReadOnly = true, Width = 200, Text = string.Format("{0}", tBlock.VertCount) }, 1, 9);


                            innerPanel.Controls.Add(new Label() { Text = "Vertex List:" }, 0, 10);

                            ListBox VertexList = new ListBox();
                            innerPanel.Controls.Add(VertexList, 0, 11);
                            innerPanel.SetColumnSpan(VertexList, 2);
                            VertexList.Dock = DockStyle.Fill;
                            VertexList.BeginUpdate();
                            tBlock.Verts.ForEach(dr => VertexList.Items.Add(dr));
                            VertexList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "Normal List:" }, 0, 12);

                            ListBox NormList = new ListBox();
                            innerPanel.Controls.Add(NormList, 0, 13);
                            innerPanel.SetColumnSpan(NormList, 2);
                            NormList.Dock = DockStyle.Fill;
                            NormList.BeginUpdate();
                            tBlock.Norms.ForEach(dr => NormList.Items.Add(dr));
                            NormList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "RGB List:" }, 0, 14);

                            ListBox RGBList = new ListBox();
                            innerPanel.Controls.Add(RGBList, 0, 15);
                            innerPanel.SetColumnSpan(RGBList, 2);
                            RGBList.Dock = DockStyle.Fill;
                            RGBList.BeginUpdate();
                            tBlock.RGBAs.ForEach(dr => RGBList.Items.Add(dr));
                            RGBList.EndUpdate();

                            innerPanel.Controls.Add(new Label() { Text = "UV List:" }, 0, 16);

                            ListBox UVList = new ListBox();
                            innerPanel.Controls.Add(UVList, 0, 17);
                            innerPanel.SetColumnSpan(UVList, 2);
                            UVList.Dock = DockStyle.Fill;
                            UVList.BeginUpdate();
                            tBlock.UVs.ForEach(dr => UVList.Items.Add(dr));
                            UVList.EndUpdate();

                            if(tBlock.ExtraData.Count > 0)
                            {
                                innerPanel.Controls.Add(new Label() { Text = "Unknown:" }, 0, 18);

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
                                innerPanel.Controls.Add(rawBox, 0, 19);
                                innerPanel.SetColumnSpan(rawBox, 2);
                                rawBox.Dock = DockStyle.Fill;
                            }
                        }

                        innerPanel.AutoSize = true;
                        innerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        pnlDetails.Controls.Add(innerPanel);
                    }
                    break;
            }
        }
    }
}
