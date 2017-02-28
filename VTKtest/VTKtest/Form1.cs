using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
        //  vtkPolyDataMapper mapper;
        vtkCellArray vertices = vtkCellArray.New();

        vtkPoints points = vtkPoints.New();
        public Form1()
        {

            InitializeComponent();
            pointPoly = vtkPolyData.New();
            //       mapper = vtkPolyDataMapper.New();

            List<vtkCellArray> pointPolylist = new List<vtkCellArray>();
            List<int> sa = new List<int>();
            for (int i = 0; i < 1; i++)
            {
                pointPolylist.Add(vtkCellArray.New());
            }
        }

        vtkUnsignedCharArray vtkcolor = vtkUnsignedCharArray.New();



        private void Form1_Load(object sender, EventArgs e)
        {
            //using (var reader = new Kitware.VTK.vtkParticleReader())
            //using (var mapper = new Kitware.VTK.vtkCompositePolyDataMapper())
            //using (var actor = new Kitware.VTK.vtkActor())
            //{
            //    // 表示させたい3Dモデルファイル
            //    reader.SetFileName(@"stHelens.particles");
            //    // Mapperにオブジェクトを写像する
            //    mapper.SetInputConnection(reader.GetOutputPort());
            //    // ActorにMapperをセットする
            //    actor.SetMapper(mapper);
            //    // 描画ウィンドウにActorを追加する
            //    renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            //}

            /*
            Random r = new Random();


            List<double[]> p = new List<double[]>();
            for (int i = 0; i < 50000; i++)
                p.Add(new double[] { r.NextDouble() * 10000, r.NextDouble() * 10000, r.NextDouble() * 10000 });

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
            */

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

            // Create a polydata object
            //  vtkPolyData pointPoly = vtkPolyData.New();

            // Set the points and vertices we created as the geometry and topology of the polydata
            pointPoly.SetPoints(points);
            pointPoly.SetVerts(vertices);


        }

        double z = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            //pointPoly.SetPoints(points);
            //pointPoly.SetVerts(vertices);

            z++;

            //double[,] p = new double[,] {
            //    {1.0, 2.0, z},
            //    {3.0, 1.0, z},
            //    {2.0, 3.0, z}
            // };

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

