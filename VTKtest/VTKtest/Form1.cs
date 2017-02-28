using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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





        private void Form1_Load(object sender, EventArgs e)
        {
            Random r = new Random();
            List<double[]> p = new List<double[]>();

            //点群を打っていく
            for (int i = 0; i < 5000000; i++)
                p.Add(new double[] { r.NextDouble() * 10000, r.NextDouble() * 10000, r.NextDouble() * 10000 });

            Addpoint(p.ToArray());


            //何してるかなぞ
            double[] bounds = pointPoly.GetBounds();

            // Find min and max z
            double minz = bounds[4];
            double maxz = bounds[5];

            //??? 
            vtkLookupTable colorLookupTable = vtkLookupTable.New();
            colorLookupTable.SetTableRange(minz, maxz);
            colorLookupTable.Build();

            //for color input 
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            int total_pt_num = pointPoly.GetNumberOfPoints();
            for (int k = 0; k < total_pt_num; k++)
            {
                double[] pp = pointPoly.GetPoint(k);
                double[] dcolor = colorLookupTable.GetColor(pp[2]);

                byte[] color = new byte[3];
                for (uint j = 0; j < 3; j++)
                    color[j] = (byte)(255 * dcolor[j]);

                colors.InsertNextTuple3(color[0], color[1], color[2]);
            }

            pointPoly.GetPointData().SetScalars(colors);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointPoly);



            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(3);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();

            renderer.SetBackground(0.3, 0.1, 0.1);



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

