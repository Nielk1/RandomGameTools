using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotHackGUCCS;
using SharpGL;
using SharpGL.Enumerations;

namespace DotHackGUCCSWinForm
{
    public partial class ModelPreview : Form
    {
        private float rotation = 0.0f;
        private float pitch = 0.0f;
        private float transX = 0.0f;
        private float transY = 0.0f;
        private double zoom = 500;
        private bool MouseLeftActive = false;
        private bool MouseRightActive = false;
        private int oldMouseX;
        private int oldMouseY;

        private CCSMeshBlock.Mesh meshData;
        private CCSMeshBlock.ShadowMesh shadowMeshData;
        private CCSMaterialBlock matBlock;
        private Bitmap image;

        uint[] textureID = new uint[1];

        public ModelPreview()
        {
            InitializeComponent();
        }

        public ModelPreview(CCSMeshBlock.Mesh meshData, CCSMaterialBlock matBlock, CCSTextureBlock textBlock, CCSPalletBlock palletBlock) : this()
        {
            this.meshData = meshData;
            this.matBlock = matBlock;
            image = textBlock != null ? (palletBlock != null ? (Bitmap)textBlock.GetImage(palletBlock.Colors) : (Bitmap)textBlock.GetImage() ) : null;

            TryLoadTexture();

            zoom = Math.Sqrt(Math.Pow(meshData.Verts.Max(dr => Math.Abs(dr.x)), 2) + Math.Pow(meshData.Verts.Max(dr => Math.Abs(dr.y)), 2) + Math.Pow(meshData.Verts.Max(dr => Math.Abs(dr.z)), 2)) * 2;
        }

        public ModelPreview(CCSMeshBlock.ShadowMesh meshData) : this()
        {
            this.shadowMeshData = meshData;

            zoom = Math.Sqrt(Math.Pow(shadowMeshData.Verts.Max(dr => Math.Abs(dr.x)), 2) + Math.Pow(shadowMeshData.Verts.Max(dr => Math.Abs(dr.y)), 2) + Math.Pow(shadowMeshData.Verts.Max(dr => Math.Abs(dr.z)), 2)) * 2;
        }

        private void TryLoadTexture()
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            if (image != null)
            {
                gl.GenTextures(1, textureID);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureID[0]);
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, /*OpenGL.GL_RGB16*/3, image.Width, image.Height, 0, OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE,
                    //image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb).Scan0);
                    image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb).Scan0);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            }
        }

        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            //  Use the 'look at' helper function to position and aim the camera.
            //gl.LookAt(0, 0, -50, 0, 0, 0, 0, 1, 0);
            gl.LookAt(0, 0, -zoom, 0, 0, 0, 0, 1, 0);

            // Rotate around X axis;
            gl.Rotate(-pitch, 1.0f, 0.0f, 0.0f);

            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            //gl.Translate(-transX, transY, 0);

            if (meshData != null)
            {
                gl.Begin(OpenGL.GL_TRIANGLES);

                gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

                //gl.PolygonMode(OpenGL.GL_FRONT, OpenGL.GL_FILL);
                //gl.PolygonMode(OpenGL.GL_BACK, OpenGL.GL_LINE);

                if (image != null)
                {
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureID[0]);
                }

                bool winding = false;
                for (int v = 0; v < meshData.Verts.Count; v++)
                {
                    if (meshData.Norms[v].w == 0)
                    {
                        if (v > 1)
                        {
                            if (winding)
                            {
                                gl.Color(meshData.RGBAs[v - 0].r / 256.0f, meshData.RGBAs[v - 0].g / 256.0f, meshData.RGBAs[v - 0].b / 256.0f, meshData.RGBAs[v - 0].a / 129.0f);
                                gl.Normal(meshData.Norms[v - 0].x / 64.0f, meshData.Norms[v - 0].y / 64.0f, -meshData.Norms[v - 0].z / 64.0f);
                                gl.TexCoord(meshData.UVs[v - 0].u / 255.0f, -meshData.UVs[v - 0].v / 255.0f);
                                gl.Vertex(meshData.Verts[v - 0].x, meshData.Verts[v - 0].y, -meshData.Verts[v - 0].z);

                                gl.Color(meshData.RGBAs[v - 1].r / 256.0f, meshData.RGBAs[v - 1].g / 256.0f, meshData.RGBAs[v - 1].b / 256.0f, meshData.RGBAs[v - 1].a / 129.0f);
                                gl.Normal(meshData.Norms[v - 1].x / 64.0f, meshData.Norms[v - 1].y / 64.0f, -meshData.Norms[v - 1].z / 64.0f);
                                gl.TexCoord(meshData.UVs[v - 1].u / 255.0f, -meshData.UVs[v - 1].v / 255.0f);
                                gl.Vertex(meshData.Verts[v - 1].x, meshData.Verts[v - 1].y, -meshData.Verts[v - 1].z);

                                gl.Color(meshData.RGBAs[v - 2].r / 256.0f, meshData.RGBAs[v - 2].g / 256.0f, meshData.RGBAs[v - 2].b / 256.0f, meshData.RGBAs[v - 2].a / 129.0f);
                                gl.Normal(meshData.Norms[v - 2].x / 64.0f, meshData.Norms[v - 2].y / 64.0f, -meshData.Norms[v - 2].z / 64.0f);
                                gl.TexCoord(meshData.UVs[v - 2].u / 255.0f, -meshData.UVs[v - 2].v / 255.0f);
                                gl.Vertex(meshData.Verts[v - 2].x, meshData.Verts[v - 2].y, -meshData.Verts[v - 2].z);

                            }
                            else
                            {
                                gl.Color(meshData.RGBAs[v - 1].r / 256.0f, meshData.RGBAs[v - 1].g / 256.0f, meshData.RGBAs[v - 1].b / 256.0f, meshData.RGBAs[v - 1].a / 129.0f);
                                gl.Normal(meshData.Norms[v - 1].x / 64.0f, meshData.Norms[v - 1].y / 64.0f, -meshData.Norms[v - 1].z / 64.0f);
                                gl.TexCoord(meshData.UVs[v - 1].u / 255.0f, -meshData.UVs[v - 1].v / 255.0f);
                                gl.Vertex(meshData.Verts[v - 1].x, meshData.Verts[v - 1].y, -meshData.Verts[v - 1].z);

                                gl.Color(meshData.RGBAs[v - 0].r / 256.0f, meshData.RGBAs[v - 0].g / 256.0f, meshData.RGBAs[v - 0].b / 256.0f, meshData.RGBAs[v - 0].a / 129.0f);
                                gl.Normal(meshData.Norms[v - 0].x / 64.0f, meshData.Norms[v - 0].y / 64.0f, -meshData.Norms[v - 0].z / 64.0f);
                                gl.TexCoord(meshData.UVs[v - 0].u / 255.0f, -meshData.UVs[v - 0].v / 255.0f);
                                gl.Vertex(meshData.Verts[v - 0].x, meshData.Verts[v - 0].y, -meshData.Verts[v - 0].z);

                                gl.Color(meshData.RGBAs[v - 2].r / 256.0f, meshData.RGBAs[v - 2].g / 256.0f, meshData.RGBAs[v - 2].b / 256.0f, meshData.RGBAs[v - 2].a / 129.0f);
                                gl.Normal(meshData.Norms[v - 2].x / 64.0f, meshData.Norms[v - 2].y / 64.0f, -meshData.Norms[v - 2].z / 64.0f);
                                gl.TexCoord(meshData.UVs[v - 2].u / 255.0f, -meshData.UVs[v - 2].v / 255.0f);
                                gl.Vertex(meshData.Verts[v - 2].x, meshData.Verts[v - 2].y, -meshData.Verts[v - 2].z);
                            }
                        }
                        winding = !winding;
                    }
                    else if (meshData.Norms[v].w == 1)
                    {
                        winding = true;
                    }
                    else if (meshData.Norms[v].w == 2)
                    {
                        winding = false;

                    }
                }

                gl.End();
            }else if(shadowMeshData != null)
            {
                gl.Begin(OpenGL.GL_TRIANGLES);

                gl.Color(1.0f, 1.0f, 1.0f, 1.0f);

                //if (image != null)
                //{
                //    gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureID[0]);
                //}

                for (int f = 0; f < shadowMeshData.Faces.Count; f+=3)
                {
                    UInt32 v0 = shadowMeshData.Faces[f + 0];
                    UInt32 v1 = shadowMeshData.Faces[f + 1];
                    UInt32 v2 = shadowMeshData.Faces[f + 2];

                    gl.Vertex(shadowMeshData.Verts[(int)v2].x, shadowMeshData.Verts[(int)v2].y, -shadowMeshData.Verts[(int)v2].z);
                    gl.Vertex(shadowMeshData.Verts[(int)v1].x, shadowMeshData.Verts[(int)v1].y, -shadowMeshData.Verts[(int)v1].z);
                    gl.Vertex(shadowMeshData.Verts[(int)v0].x, shadowMeshData.Verts[(int)v0].y, -shadowMeshData.Verts[(int)v0].z);
                }

                gl.End();

                gl.Begin(OpenGL.GL_LINES);

                gl.Color(0.0f, 0.0f, 0.0f, 1.0f);

                for (int f = 0; f < shadowMeshData.Faces.Count; f += 3)
                {
                    UInt32 v0 = shadowMeshData.Faces[f + 0];
                    UInt32 v1 = shadowMeshData.Faces[f + 1];
                    UInt32 v2 = shadowMeshData.Faces[f + 2];

                    gl.Vertex(shadowMeshData.Verts[(int)v0].x, shadowMeshData.Verts[(int)v0].y, -shadowMeshData.Verts[(int)v0].z);
                    gl.Vertex(shadowMeshData.Verts[(int)v2].x, shadowMeshData.Verts[(int)v2].y, -shadowMeshData.Verts[(int)v2].z);

                    gl.Vertex(shadowMeshData.Verts[(int)v2].x, shadowMeshData.Verts[(int)v2].y, -shadowMeshData.Verts[(int)v2].z);
                    gl.Vertex(shadowMeshData.Verts[(int)v1].x, shadowMeshData.Verts[(int)v1].y, -shadowMeshData.Verts[(int)v1].z);

                    gl.Vertex(shadowMeshData.Verts[(int)v1].x, shadowMeshData.Verts[(int)v1].y, -shadowMeshData.Verts[(int)v1].z);
                    gl.Vertex(shadowMeshData.Verts[(int)v0].x, shadowMeshData.Verts[(int)v0].y, -shadowMeshData.Verts[(int)v0].z);
                }

                gl.End();
            }
        }

        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_LIGHTING);

            // alpha vertex colors
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            // 1 sided faces
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.Enable(OpenGL.GL_LIGHT0);

            //gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, new float[] { 0.4f, 0.4f, 0.4f });
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, new float[] { 0.75f, 0.75f, 0.75f });

            float[] light_position = { 0, -1, 0, 0 };
            float[] light_brightness = { 1, 1, 1, 1 };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light_position);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light_brightness);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light_brightness);
        }

        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(60.0f, (double)openGLControl.Width / (double)openGLControl.Height, 1, 100000.0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom += (e.Delta * -0.5);
            zoom = Math.Min(100000.0d, Math.Max(zoom, 0.00001d));
            //Console.WriteLine(zoom);
        }

        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                oldMouseX = e.X;
                oldMouseY = e.Y;
                MouseLeftActive = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                oldMouseX = e.X;
                oldMouseY = e.Y;
                MouseRightActive = true;
            }
        }

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseLeftActive)
            {
                if (e.Button == MouseButtons.Left)
                {
                    rotation += ((e.X - oldMouseX) / (float)openGLControl.Width * 250f);
                    oldMouseX = e.X;

                    pitch += ((e.Y - oldMouseY) / (float)openGLControl.Height * 250f);
                    oldMouseY = e.Y;
                }
                else
                {
                    MouseLeftActive = false;
                }
            }
            if (MouseRightActive)
            {
                if (e.Button == MouseButtons.Right)
                {
                    transX += ((e.X - oldMouseX) / (float)openGLControl.Width * 25f);
                    oldMouseX = e.X;

                    transY += ((e.Y - oldMouseY) / (float)openGLControl.Height * 25f);
                    oldMouseY = e.Y;
                }
                else
                {
                    MouseRightActive = false;
                }
            }
        }

        private void openGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseLeftActive = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                MouseRightActive = false;
            }
        }
    }
}
