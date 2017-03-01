using Kitware.VTK;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        vtkPolyData pointPoly;
        //vtkPolyDataMapper mapper;
        vtkCellArray vertices = vtkCellArray.New();
        vtkPoints points = vtkPoints.New();

        public Form1()
        {
            InitializeComponent();
            pointPoly = vtkPolyData.New();

            //mapper = vtkPolyDataMapper.New();
            List<vtkCellArray> pointPolylist = new List<vtkCellArray>();

            for (int i = 0; i < 1; i++)
                pointPolylist.Add(vtkCellArray.New());

        }


        private void Form1_Load(object sender, EventArgs e)
        {

            


            Random r = new Random();
            List<double[]> p = new List<double[]>();

            //for color input 
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");


            //file manage
            
            string analysis_target = @"C:\00_Share\01_AndeuxWorks\ver20170301\15A24\161222161214";

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(analysis_target);
            IEnumerable<System.IO.FileInfo> files =
                di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);
            //ファイルを列挙する
            foreach (System.IO.FileInfo f in files)
            {
            
                int z = 0;
                if (f.Extension == ".bmp")
                {
                    Console.WriteLine("{0}/{1}", f.FullName, f.Extension);
                    Mat img = Cv2.ImRead(f.FullName);

                    //string file = @"C:\00_Share\01_AndeuxWorks\ver20170301\15A24\161222161214\904.bmp";
                    //Mat img = Cv2.ImRead(file);
                    z = Int32.Parse(Path.GetFileNameWithoutExtension(@f.FullName));

                    for (int y = 0; y < img.Height; y++)
                    {
                        for (int x = 0; x < img.Width; x++)
                        {
                            Vec3b pix_put = img.At<Vec3b>(y, x);

                            int B = pix_put[0];
                            int G = pix_put[1];
                            int R = pix_put[2];

                            if (x> 85 && B>70 && G >70 && R >70)
                            {
                                p.Add(new double[] { x, y, z });
                                if (B > 120)
                                {
                                    colors.InsertNextTuple3(255, 0, 0);//R,G,B

                                }
                                else
                                {
                                    colors.InsertNextTuple3(R, G, B);//R,G,B

                                }

                            }

                        }
                    }
                }
                //Console.WriteLine("{0}",z);
            }


            //Mat img = Cv2.ImRead("");
            //点群を打っていく
            /*
            for (int i = 0; i < 5000000; i++)
            {
                p.Add(new double[] { r.NextDouble() * 10000, r.NextDouble() * 10000, r.NextDouble() * 10000 });
                colors.InsertNextTuple3(0, 0, 255);//R,G,B
            }
            */
            //ポイントを反映
            Addpoint(p.ToArray());

            //色情報を挿入
            pointPoly.GetPointData().SetScalars(colors);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointPoly);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(3);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();

            renderer.SetBackground(0.0, 0.1, 0.3);
            renderer.AddActor(actor);

            Console.WriteLine(z);
        }


        private void Addpoint(double[][] p)
        {           
            // Create topology of the points (a vertex per point)
            int nPts = p.Length;
            int[] ids = new int[nPts];

            for (int i = 0; i < nPts; i++)
                ids[i] = points.InsertNextPoint(p[i][0], p[i][1], p[i][2]);

            int size = Marshal.SizeOf(typeof(int)) * nPts;
            IntPtr pIds = Marshal.AllocHGlobal(size);
            Marshal.Copy(ids, 0, pIds, nPts);
            vertices.InsertNextCell(nPts, pIds);
            Marshal.FreeHGlobal(pIds);

            pointPoly.SetPoints(points);
            pointPoly.SetVerts(vertices);
        }

        double z = 0;



        private void timer1_Tick(object sender, EventArgs e)
        {
            //pointPoly.SetPoints(points);
            //pointPoly.SetVerts(vertices);
            z++;
            Random r = new Random();
            List<double[]> p = new List<double[]>();
            for (int i = 0; i < 1000000; i++)
                p.Add(new double[] { r.NextDouble() * 100, r.NextDouble() * 100, r.NextDouble() * 100 });
            Addpoint(p.ToArray());
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointPoly);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(3);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.3, 0.2, 0.1);
            renderer.AddActor(actor);
            Console.WriteLine(z);
        }

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {

        }

    }

}

