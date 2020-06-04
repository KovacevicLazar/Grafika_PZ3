using grafikaPZ3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace pz3_Grafika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const double minLongitude = 19.793909;
        public const double maxLongitude = 19.894459;

        public const double minLatitude = 45.2325;
        public const double maxLatitude = 45.277031;


        private System.Windows.Point start = new System.Windows.Point();
        private System.Windows.Point diffOffset = new System.Windows.Point();
        private int zoomMax = 50;
        private int zoomCurent = 30;
        private bool MiddleClicked = false;


        DoubleAnimation doubleAnimation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(15));
        
        //point.x, ObjectCnt
        private static Dictionary<System.Windows.Point, int> numberOfEntityOnPoint = new Dictionary<System.Windows.Point, int>();

        private static Dictionary<long, PowerEntity> collectionOfNodesID = new Dictionary<long, PowerEntity>();

        List<GeometryModel3D> SwitchGeometryModels = new List<GeometryModel3D>();
        List<GeometryModel3D> NodeGeometryModels = new List<GeometryModel3D>();
        List<GeometryModel3D> SubstationGeometryModels = new List<GeometryModel3D>();

        public XmlEntities xmlEntities { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.xmlEntities = ParseXml();

            foreach (var node in xmlEntities.Nodes)
            {
                if (node.Longitude == 0 || node.Latitude == 0 )
                {
                    continue; 
                }
                //collectionOfNodesID.Add(node.Id, node);

                MeshGeometry3D meshGeometry3D = createCubeMeshGeometry(node.Longitude, node.Latitude, node.Id);

                DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Red));

                GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);


                RotateTransform3D rotateTransform3D = new RotateTransform3D();

                rotateTransform3D.Rotation = myAngleRotation;


                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);


                geometryModel3D.Transform = myTransform3DGroup;


                NodeGeometryModels.Add(geometryModel3D);
                model3dGroup.Children.Add(geometryModel3D);


            }

            foreach (var sub in xmlEntities.Substations)
            {

                if (sub.Longitude == 0 || sub.Latitude == 0 )
                {
                    continue;
                }
                //collectionOfNodesID.Add(sub.Id, sub);

                MeshGeometry3D meshGeometry3D = createCubeMeshGeometry(sub.Longitude, sub.Latitude, sub.Id);

                DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));

                GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);


                RotateTransform3D rotateTransform3D = new RotateTransform3D();

                rotateTransform3D.Rotation = myAngleRotation;


                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);


                geometryModel3D.Transform = myTransform3DGroup;

                SubstationGeometryModels.Add(geometryModel3D);
                model3dGroup.Children.Add(geometryModel3D);

            }

            foreach (var sw in xmlEntities.Switches)
            {
                if (sw.Longitude == 0 || sw.Latitude == 0 )
                {
                    continue;
                }

                //collectionOfNodesID.Add(sw.Id, sw);

                MeshGeometry3D meshGeometry3D = createCubeMeshGeometry(sw.Longitude, sw.Latitude, sw.Id);

                DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Green));

                GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);
                

                RotateTransform3D rotateTransform3D = new RotateTransform3D();

                rotateTransform3D.Rotation = myAngleRotation;


                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);


                geometryModel3D.Transform = myTransform3DGroup;

                SwitchGeometryModels.Add(geometryModel3D);
                model3dGroup.Children.Add(geometryModel3D);

            }

            foreach(var line in xmlEntities.Lines)
            {
                System.Windows.Point point1 = new System.Windows.Point();
                System.Windows.Point point2 = new System.Windows.Point();
                var cnt = 1;
                System.Windows.Point Startpoint = new System.Windows.Point();
                System.Windows.Point Endpoint = new System.Windows.Point();

                if(collectionOfNodesID.ContainsKey(line.FirstEnd) && collectionOfNodesID.ContainsKey(line.SecondEnd))
                {
                    Startpoint = CreatePoint(collectionOfNodesID[line.FirstEnd].Longitude, collectionOfNodesID[line.FirstEnd].Latitude);
                    Endpoint = CreatePoint(collectionOfNodesID[line.SecondEnd].Longitude, collectionOfNodesID[line.SecondEnd].Latitude);
               
                }
                else
                {
                    continue; //Preskoci vod alo povezuje cvorove koji nisu na nasoj mapi, ovo je obezbedjeno vec pri citanju iz xml
                }

                foreach (var point in line.Vertices)
                {
                    if (cnt == 1)
                    {
                        point1 = CreatePoint(point.Longitude, point.Latitude);
                        model3dGroup.Children.Add(createLineGeometryModel3D(Startpoint, point1,myAngleRotation));
                        cnt++;
                        continue;
                    }
                    else if (cnt == 2)
                    {
                        point2 = CreatePoint(point.Longitude, point.Latitude);
                        model3dGroup.Children.Add(createLineGeometryModel3D(point1, point2, myAngleRotation));
                        point1 = point2;
                    }


                }
          
                model3dGroup.Children.Add(createLineGeometryModel3D(point2, Endpoint, myAngleRotation));


            }
            
        }

        private static GeometryModel3D createLineGeometryModel3D(System.Windows.Point point1, System.Windows.Point point2, AxisAngleRotation3D myAngleRotation)
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(point1.X+1, point1.Y+1, 0));
            points.Add(new Point3D(point1.X, point1.Y, 6));
            points.Add(new Point3D(point2.X+1, point2.Y+1, 0));
            points.Add(new Point3D(point2.X, point2.Y, 6));

            points.Add(new Point3D(point1.X - 1, point1.Y - 1, 0));
            points.Add(new Point3D(point1.X, point1.Y, 6));
            points.Add(new Point3D(point2.X - 1, point2.Y - 1, 0));
            points.Add(new Point3D(point2.X , point2.Y, 6));

            meshGeometry3D.Positions = new Point3DCollection(points);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(1);


            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(7);


             meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(3);


            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(5);

            DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Black));

            GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);



            RotateTransform3D rotateTransform3D = new RotateTransform3D();

            rotateTransform3D.Rotation = myAngleRotation;


            Transform3DGroup myTransform3DGroup = new Transform3DGroup();
            myTransform3DGroup.Children.Add(rotateTransform3D);


            geometryModel3D.Transform = myTransform3DGroup;

            return geometryModel3D;
        }



        private static MeshGeometry3D createCubeMeshGeometry(double Longitude, double Latitude, long EntityID)
        {
            var point = CreatePoint(Longitude, Latitude);

            int numberOfEntity = 0;

            if (numberOfEntityOnPoint.ContainsKey(point))
            {
                numberOfEntityOnPoint[point]++;
                numberOfEntity = numberOfEntityOnPoint[point];
            }
            else
            {
                numberOfEntityOnPoint[point] = 1;
                numberOfEntity = 1;
            }

            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(point.X - 4, point.Y - 4, (numberOfEntity) * 10 - 10));
            points.Add(new Point3D(point.X + 4, point.Y - 4, (numberOfEntity) * 10 - 10));
            points.Add(new Point3D(point.X + 4, point.Y - 4, (numberOfEntity) * 10));
            points.Add(new Point3D(point.X - 4, point.Y - 4, (numberOfEntity ) * 10));

            points.Add(new Point3D(point.X - 4, point.Y + 4, (numberOfEntity ) * 10));
            points.Add(new Point3D(point.X + 4, point.Y + 4, (numberOfEntity ) * 10));

            points.Add(new Point3D(point.X + 4, point.Y + 4, (numberOfEntity ) * 10 - 10));

            points.Add(new Point3D(point.X - 4, point.Y + 4, (numberOfEntity ) * 10 - 10));


            meshGeometry3D.Positions = new Point3DCollection(points);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(3);


            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(5);

            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(6);

            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(3);

            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(6);



            return meshGeometry3D;
        }



        private static System.Windows.Point CreatePoint(double longitude, double latitude)
        {
            double ValueoOfOneLongitude = (maxLongitude - minLongitude) / 1175; //pravimo 2200delova (Longituda) jer nam je canvas 2200x2200 
            double ValueoOfOneLatitude = (maxLatitude - minLatitude) / 775;  //isto kao gore
           
            double x = Math.Round((longitude - minLongitude) / ValueoOfOneLongitude)- 587.5; // koliko longituda stane u rastojanje izmedju trenutne i minimalne longitude
            double y = Math.Round((latitude - minLatitude) / ValueoOfOneLatitude) - 387.5;

            x = x - x % 8; // zaokruzi na prvi broj deljiv sa 10, toliko ce nam biti rastojanje izmedju dva susedna x
            y = y - y % 8; // zaokruzi na prvi broj deljiv sa 10,, toliko ce nam biti rastojanje izmedju dva susedna y


            return new System.Windows.Point
            {
                X = x,
                Y = y,
            };
        }




        public static XmlEntities ParseXml()
        {
            XmlEntities xmlEntities = new XmlEntities();
            var filename = "Geographic.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = System.IO.Path.Combine(currentDirectory, filename);

            StringBuilder result = new StringBuilder();

            //Load xml
            XDocument xdoc = XDocument.Load(filename);

            //Run query
            var substations = xdoc.Descendants("SubstationEntity")
                     .Select(sub => new SubstationEntity
                     {
                         Id = (long)sub.Element("Id"),
                         Name = (string)sub.Element("Name"),
                         X = (double)sub.Element("X"),
                         Y = (double)sub.Element("Y"),
                     }).ToList();

            double longit = 0;
            double latid = 0;

            for (int i = 0; i < substations.Count(); i++)
            {
                var item = substations[i];
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                if (latid <= minLatitude || latid >= maxLatitude || longit <= minLongitude || longit >= maxLongitude)
                {
                    substations.RemoveAt(i);
                    continue;
                }
                else
                {
                    substations[i].Latitude = latid;
                    substations[i].Longitude = longit;
                    xmlEntities.Substations.Add(substations[i]);
                    collectionOfNodesID.Add(substations[i].Id, substations[i]);
                }
               
            }

            var nodes = xdoc.Descendants("NodeEntity")
                     .Select(node => new NodeEntity
                     {
                         Id = (long)node.Element("Id"),
                         Name = (string)node.Element("Name"),
                         X = (double)node.Element("X"),
                         Y = (double)node.Element("Y"),
                     }).ToList();

            for (int i = 0; i < nodes.Count(); i++)
            {
                var item = nodes[i];
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                if (latid <= minLatitude || latid >= maxLatitude || longit <= minLongitude || longit >= maxLongitude)
                {
                    nodes.RemoveAt(i);
                    continue;
                }
                else
                {
                    nodes[i].Latitude = latid;
                    nodes[i].Longitude = longit;
                    xmlEntities.Nodes.Add(nodes[i]);
                    collectionOfNodesID.Add(nodes[i].Id, nodes[i]);
                }
            }

            var switches = xdoc.Descendants("SwitchEntity")
                     .Select(sw => new SwitchEntity
                     {
                         Id = (long)sw.Element("Id"),
                         Name = (string)sw.Element("Name"),
                         Status = (string)sw.Element("Status"),
                         X = (double)sw.Element("X"),
                         Y = (double)sw.Element("Y"),
                     }).ToList();

            for (int i = 0; i < switches.Count(); i++)
            {
                var item = switches[i];
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                if (latid <= minLatitude || latid >= maxLatitude || longit <= minLongitude || longit >= maxLongitude)
                {
                    switches.RemoveAt(i);
                    continue;
                }
                else
                {
                    switches[i].Latitude = latid;
                    switches[i].Longitude = longit;
                    xmlEntities.Switches.Add(switches[i]);
                    collectionOfNodesID.Add(switches[i].Id, switches[i]);
                }
            }

            var lines = xdoc.Descendants("LineEntity")
                     .Select(line => new LineEntity
                     {
                         Id = (long)line.Element("Id"),
                         Name = (string)line.Element("Name"),
                         ConductorMaterial = (string)line.Element("ConductorMaterial"),
                         IsUnderground = (bool)line.Element("IsUnderground"),
                         R = (float)line.Element("R"),
                         FirstEnd = (long)line.Element("FirstEnd"),
                         SecondEnd = (long)line.Element("SecondEnd"),
                         LineType = (string)line.Element("LineType"),
                         ThermalConstantHeat = (long)line.Element("ThermalConstantHeat"),
                         Vertices = line.Element("Vertices").Descendants("Point").Select(p => new grafikaPZ3.Model.Point
                         {
                             X = (double)p.Element("X"),
                             Y = (double)p.Element("Y"),
                         }).ToList()
                     }).ToList();

            for (int i = 0; i < lines.Count(); i++)
            {
                if(collectionOfNodesID.ContainsKey(lines[i].SecondEnd) && collectionOfNodesID.ContainsKey(lines[i].FirstEnd))
                {
                    var line = lines[i];
                    foreach (var point in line.Vertices)
                    {

                        ToLatLon(point.X, point.Y, 34, out latid, out longit);
                        point.Latitude = latid;
                        point.Longitude = longit;

                    }
                    xmlEntities.Lines.Add(line);
                }
            }

            return xmlEntities;
        }


        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        private void ViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ViewPort.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetY;

        }

        private void ViewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewPort.ReleaseMouseCapture();
        }

        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {

            if (ViewPort.IsMouseCaptured)
            {
                System.Windows.Point currentPosition = e.GetPosition(this);
                double offsetX = currentPosition.X - start.X;
                double offsetY = currentPosition.Y - start.Y;
             
                double translateX = -(offsetX)/2; //Usporavam kretanje za duplo
                double translateY = (offsetY)/2;
                translacija.OffsetX = diffOffset.X + translateX;
                translacija.OffsetY = diffOffset.Y + translateY;
            }
            if (MiddleClicked)
            {
                System.Windows.Point currentPosition = e.GetPosition(this);
                double offsetX = currentPosition.X - start.X;
                double offsetY = currentPosition.Y - start.Y;

                if (offsetX < 0)
                {
                    doubleAnimation.To = -360;
                    myAngleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, doubleAnimation);
                    MiddleClicked = false;
                }
                else
                {
                    doubleAnimation.To = 360;
                    myAngleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, doubleAnimation);
                    MiddleClicked = false;
                }
               
            }

        }

        private void ViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point3D p = new Point3D( CAMERA.Position.X, CAMERA.Position.Y,CAMERA.Position.Z);
         
            if (e.Delta > 0 && zoomCurent > -zoomMax)
            {
                translacija.OffsetZ -= 10;
                zoomCurent--;
            }
            else if (e.Delta <= 0 && zoomCurent < zoomMax)
            {  
                translacija.OffsetZ += 10;
                zoomCurent++;
            }
        }

        private void ViewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                MiddleClicked = true;
                doubleAnimation.To = 360;

                start = e.GetPosition(this);
                diffOffset.X = translacija.OffsetX;
                diffOffset.Y = translacija.OffsetY;

            }
        }

        private void ViewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Released)
            {
                MiddleClicked = false;
                doubleAnimation.From = myAngleRotation.Angle;
                doubleAnimation.To = myAngleRotation.Angle;
                myAngleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, doubleAnimation);
            }
        }

       


        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.Content.ToString() == "Switchs")
            {
                foreach (var geoModel in SwitchGeometryModels)
                {
                    model3dGroup.Children.Remove(geoModel);
                }

            }
            else if (checkBox.Content.ToString() == "Nodes")
            {
                foreach (var geoModel in NodeGeometryModels)
                {
                    model3dGroup.Children.Remove(geoModel);
                }

            }
            else if (checkBox.Content.ToString() == "Substations")
            {
                foreach (var geoModel in SubstationGeometryModels)
                {
                    model3dGroup.Children.Remove(geoModel);
                }

            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
          
            if(checkBox.Content.ToString() == "Switchs")
            {
                foreach(var geoModel in SwitchGeometryModels)
                {
                    model3dGroup.Children.Add(geoModel);
                }

            }
            else if (checkBox.Content.ToString() == "Nodes")
            {
                foreach (var geoModel in NodeGeometryModels)
                {
                    model3dGroup.Children.Add(geoModel);
                }

            }
            else if (checkBox.Content.ToString() == "Substations")
            {
                foreach (var geoModel in SubstationGeometryModels)
                {
                    model3dGroup.Children.Add(geoModel);
                }

            }
        }
    }
}
