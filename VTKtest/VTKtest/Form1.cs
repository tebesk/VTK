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

            //vtkDataSet dataseting = vtkDataSet.New()

            /*

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
                    Mat oneimg = Cv2.ImRead(f.FullName);
            
            */


            ///////vtk voxel -----------------------------------------------
            vtkImageData imgData = vtkImageData.New();
            vtkUnsignedCharArray pixel = vtkUnsignedCharArray.New();

            string file = @"C:\00_Share\01_AndeuxWorks\ver20170301\15A24\161222161214\904.bmp";
            Mat img = Cv2.ImRead(file);

            imgData.SetDimensions(600, 521, 160);
            imgData.SetScalarTypeToUnsignedChar();
            imgData.SetNumberOfScalarComponents(4);
            for (int i = 0; i < 160; i++)
            {


                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {


                        Vec3b pix_put = img.At<Vec3b>(y, x);

                        int B = pix_put[0];
                        int G = pix_put[1];
                        double R = pix_put[2];
                        if (B>70)
                        {
                            imgData.SetScalarComponentFromDouble(x, y, i, 2, R);

                        }
                    }
                }
            }



            vtkVolumeRayCastCompositeFunction compositefunc = vtkVolumeRayCastCompositeFunction.New();
            vtkVolumeRayCastMapper volumeMapper = vtkVolumeRayCastMapper.New();
            volumeMapper.SetVolumeRayCastFunction(compositefunc);
            volumeMapper.SetInput(imgData);

            //color setting
            vtkColorTransferFunction colorTransferFunction = vtkColorTransferFunction.New();
            colorTransferFunction.AddRGBPoint(0.0, 0.0, 0.0, 0.0);
            colorTransferFunction.AddRGBPoint(55.0, 0.0, 0.0, 0.0);
            colorTransferFunction.AddRGBPoint(70.0, 0.0, 1.0, 0.0);
            colorTransferFunction.AddRGBPoint(100.0, 0.0, 1.0, 0.0);
            colorTransferFunction.AddRGBPoint(150.0, 1.0, 0.0, 0.0);
            colorTransferFunction.AddRGBPoint(250.0, 1.0, 0.0, 0.0);
            //透明化
            vtkPiecewiseFunction opacityTransferFunction = vtkPiecewiseFunction.New();
            opacityTransferFunction.AddPoint(0, 0.0);
            opacityTransferFunction.AddPoint(55, 0.1);
            opacityTransferFunction.AddPoint(100, 0.8);
            opacityTransferFunction.AddPoint(150, 1.0);

            vtkVolumeProperty volumeProperty = vtkVolumeProperty.New();
            volumeProperty.SetColor(colorTransferFunction);
            volumeProperty.SetScalarOpacity(opacityTransferFunction);
            volumeProperty.ShadeOn();
            volumeProperty.SetInterpolationTypeToLinear();

            vtkVolume volume = vtkVolume.New();
            volume.SetMapper(volumeMapper);
            volume.SetProperty(volumeProperty);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.0, 0.1, 0.1);
            renderer.AddVolume(volume);            

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

