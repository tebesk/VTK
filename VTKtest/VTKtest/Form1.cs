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


        private void Form1_Load(object sender, EventArgs e)
        {
            vtkPoints points = vtkPoints.New();
            uint GridSize = 20;
            double xx, yy, zz;
            for (uint x = 0; x < GridSize; x++)
            {
                for (uint y = 0; y < GridSize; y++)
                {
                    xx = x + vtkMath.Random(-.2, .2);
                    yy = y + vtkMath.Random(-.2, .2);
                    zz = vtkMath.Random(-.5, .5);
                    points.InsertNextPoint(xx, yy, zz);
                }
            }

            // Add the grid points to a polydata object
            vtkPolyData inputPolyData = vtkPolyData.New();
            inputPolyData.SetPoints(points);

            // Triangulate the grid points
            vtkDelaunay2D delaunay = vtkDelaunay2D.New();

            #if VTK_MAJOR_VERSION_5
                delaunay.SetInput(inputPolyData);
            #else
                delaunay.SetInput(inputPolyData);
            #endif
            delaunay.Update();
            vtkPolyData outputPolyData = delaunay.GetOutput();

            double[] bounds = outputPolyData.GetBounds();

            // Find min and max z
            double minz = bounds[4];
            double maxz = bounds[5];

            Debug.WriteLine("minz: " + minz);
            Debug.WriteLine("maxz: " + maxz);

            // Create the color map
            vtkLookupTable colorLookupTable = vtkLookupTable.New();
            colorLookupTable.SetTableRange(minz, maxz);
            colorLookupTable.Build();

            // Generate the colors for each point based on the color map
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            Debug.WriteLine("There are " + outputPolyData.GetNumberOfPoints()
                      + " points.");


#if UNSAFE // fastest way to fill color array
         colors.SetNumberOfTuples(outputPolyData.GetNumberOfPoints());
         unsafe {
            byte* pColor = (byte*)colors.GetPointer(0).ToPointer();
 
            for(int i = 0; i < outputPolyData.GetNumberOfPoints(); i++) {
               double[] p = outputPolyData.GetPoint(i);
 
               double[] dcolor = colorLookupTable.GetColor(p[2]);
               Debug.WriteLine("dcolor: "
                         + dcolor[0] + " "
                         + dcolor[1] + " "
                         + dcolor[2]);
 
               byte[] color = new byte[3];
               for(uint j = 0; j < 3; j++) {
                  color[j] = (byte)( 255 * dcolor[j] );
               }
               Debug.WriteLine("color: "
                         + color[0] + " "
                         + color[1] + " "
                         + color[2]);
 
               *( pColor + 3 * i ) = color[0];
               *( pColor + 3 * i + 1 ) = color[1];
               *( pColor + 3 * i + 2 ) = color[2];
            }
         }
#else
            for (int i = 0; i < outputPolyData.GetNumberOfPoints(); i++)
            {
                double[] p = outputPolyData.GetPoint(i);

                double[] dcolor = colorLookupTable.GetColor(p[2]);
                Debug.WriteLine("dcolor: "
                          + dcolor[0] + " "
                          + dcolor[1] + " "
                          + dcolor[2]);

                byte[] color = new byte[3];
                for (uint j = 0; j < 3; j++)
                {
                    color[j] = (byte)(255 * dcolor[j]);
                }
                Debug.WriteLine("color: "
                          + color[0] + " "
                          + color[1] + " "
                          + color[2]);
                colors.InsertNextTuple3(color[0], color[1], color[2]);
                //IntPtr pColor = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * 3);
                //Marshal.Copy(color, 0, pColor, 3);
                //colors.InsertNextTupleValue(pColor);
                //Marshal.FreeHGlobal(pColor);
            }
#endif

            outputPolyData.GetPointData().SetScalars(colors);

            // Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
         mapper.SetInputConnection(outputPolyData.GetProducerPort());
#else
            mapper.SetInput(outputPolyData);
#endif

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);




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

