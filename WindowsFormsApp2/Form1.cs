using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Windows;
using Obstacle;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        //DISCLAIMER
        //All shown points on screen must be PointF instead of Point
        //This is because of how the rotation works in the code
        // same must be done for rectangles if the are moving relative to the position of the player


        Rectangle player1 = new Rectangle(200, 300, 60, 60);
        RectangleF cone = new Rectangle(1, 1, 400, 300);

        PointF intersectionPoint = new PointF(1, 1);

        List<PointF> pointsSQ = new List<PointF>();

        bool firstrunthroughOfScan = false;

        double rotateSpeed = 2;
        //
        int switchListCounter = 1; // This is to tell the code which point group to add points to
        //
        PointF pivotPoint = new PointF(100F, 100F);
        double rotationSpin = 0D;

        PointF coneProjectionPoint = new PointF();

        List<PointF> viewPoints = new List<PointF>();
        List<PointF> drawnViewPoints = new List<PointF>();

        //These are the current obsticles/walls shown in the game
        //
        PointF[] pointsSQ1 = new PointF[3];
        PointF[] pointsSQ2 = new PointF[3];
        PointF[] pointsSQ3 = new PointF[3];
        //

        PointF[] viewPointsArray = new PointF[20];
        PointF[] viewConeArray = new PointF[5];

        List<PointF> closestPoints = new List<PointF>();

        List<PointF> pointsLinetest = new List<PointF>();

        List<PointF> containsPoints = new List<PointF>();

        //These are lists that contain groups of points that are far enought away from enother to be different objects
        //More of these may need to be added if enviroments get more complex
        //If more point groups are added, remember to add them to the vision coed and other code that uses them
        //
        List<PointF> pointGroup1 = new List<PointF>();
        List<PointF> pointGroup2 = new List<PointF>();
        List<PointF> pointGroup3 = new List<PointF>();
        List<PointF> pointGroup4 = new List<PointF>();
        List<PointF> pointGroup5 = new List<PointF>();
        List<PointF> pointGroup6 = new List<PointF>();
        List<PointF> pointGroup7 = new List<PointF>();
        List<PointF> pointGroup8 = new List<PointF>();
        //

        //This list contains the booolian of if a line has allready encountered another line and that it has added it's intersection point to one of the point groups
        //
        List<bool> lineEnabled = new List<bool>();
        //

        List<List<PointF>> obsticalsTSSl = new List<List<PointF>>(); //obsticles that stop sightlines
        List<List<PointF>> everyObjectThatMoves = new List<List<PointF>>();
        float speed = 8f;

        bool wDown = false;
        bool sDown = false;
        bool aDown = false;
        bool dDown = false;
        bool qDown = false;
        bool eDown = false;

        List<double> scanLines = new List<double>();

        List<PointF> correctDistanceTester = new List<PointF>();

        int rotationAngle = 0;

        SolidBrush blueBrush = new SolidBrush(Color.DodgerBlue);
        Pen whitePen = new Pen(Color.White, 2);
        Pen greenPen = new Pen(Color.Green, 10);
        Pen orangePen = new Pen(Color.Orange, 5);

        List<List<PointF>> visableObjects = new List<List<PointF>>();
        List<List<PointF>> visableObjectsMk2 = new List<List<PointF>>();

        Region visionConeR = new Region();
        public Form1()
        {
            InitializeComponent();
            cone = new RectangleF(player1.Width + player1.X - 200, (player1.Height / 2) + player1.X - 170, 700, 500);
            // This is where all visable points will be placed
            //pointsSQ3[0] = new PointF(500, 300);
            //pointsSQ3[1] = new PointF(450, 350);
            //pointsSQ3[2] = new PointF(500, 400);
            //List<PointF> pointsSQ3List = pointsSQ3.ToList();

            pointsSQ2[0] = new PointF(700, 150);
            pointsSQ2[1] = new PointF(650, 200);
            pointsSQ2[2] = new PointF(700, 250);
            List<PointF> pointsSQ2List = pointsSQ2.ToList();

            pointsSQ1[0] = new PointF(500, 100);
            pointsSQ1[1] = new PointF(450, 150);
            pointsSQ1[2] = new PointF(500, 200);
            List<PointF> pointsSQ1List = pointsSQ1.ToList();

            //everyObjectThatMoves.Add(pointsSQ3List);
            everyObjectThatMoves.Add(pointsSQ2List);
            everyObjectThatMoves.Add(pointsSQ1List);

            //obsticalsTSSl.Add(pointsSQ3List);
            obsticalsTSSl.Add(pointsSQ2List);
            obsticalsTSSl.Add(pointsSQ1List);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.Q:
                    qDown = true;
                    break;
                case Keys.E:
                    eDown = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
                case Keys.Q:
                    qDown = false;
                    break;
                case Keys.E:
                    eDown = false;
                    break;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            // Get the graphics object from the PaintEventArgs
            Graphics g = e.Graphics;

            // Set the image interpolation mode to high quality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Create a color matrix for the color distortion effect
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                new float[] { 2f, 0, 0, 0, 0 },
                new float[] { 0, 5f, 0, 0, 0 },
                new float[] { 1, 0, 1.5f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { -0.5f, -0.5f, -0.5f, 0, 1 }
            });

            // Create an image attributes object and set the color matrix
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            // Draw the objects on the screen using the color matrix
            Point[] points = { new Point(10, 10), new Point(20, 20), new Point(30, 10) };
            g.DrawPolygon(new Pen(Color.Orange), points);
            for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
            {
                PointF[] temp3 = new PointF[3];
                temp3 = everyObjectThatMoves[i].ToArray();
                g.DrawLines(greenPen, temp3);
            }
            e.Graphics.DrawPolygon(whitePen, viewPointsArray);
            e.Graphics.FillRectangle(blueBrush, player1);
            e.Graphics.DrawRectangle(orangePen, new Rectangle(Convert.ToInt32(cone.X), Convert.ToInt32(cone.Y), Convert.ToInt32(cone.Width), Convert.ToInt32(cone.Height)));
            PointF[] transfer = new PointF[correctDistanceTester.Count];
            correctDistanceTester.CopyTo(transfer);
            if (transfer.Length >= 2)
            {
                e.Graphics.DrawPolygon(orangePen, transfer);
            }
            if (transfer.Length == 1)
            {
                e.Graphics.DrawLine(orangePen, transfer[0], coneProjectionPoint);
            }
            // Draw the scan lines
            Pen scanLinePen = new Pen(Color.Black);
            int scanLineHeight = 2;
            for (int y = 0; y < this.Height; y += scanLineHeight)
            {
                g.DrawLine(scanLinePen, 0, y, this.Width, y);
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
            //{
            //    Graphics g = this.CreateGraphics();
            //    PointF[] temp3 = new PointF[3];
            //    temp3 = everyObjectThatMoves[i].ToArray();
            //    g.DrawLines(greenPen, temp3);
            //}
            ////e.Graphics.DrawRectangle(orangePen, cone);
            ////Obstical();
            ////Obstical2();
            ////Obstical3();
            ////e.Graphics.DrawLines(whitePen, pointsLinetest);
            //e.Graphics.FillRectangle(blueBrush, player1);
            ////e.Graphics.DrawPolygon(greenPen, viewConeArray);
            //e.Graphics.DrawPolygon(whitePen, viewPointsArray);
            ////e.Graphics.DrawClosedCurve(whitePen, viewPointsArray);
            //e.Graphics.DrawRectangle(orangePen, new Rectangle(Convert.ToInt32(cone.X), Convert.ToInt32(cone.Y), Convert.ToInt32(cone.Width), Convert.ToInt32(cone.Height)));
            //viewPoints.Clear();
            //e.Graphics.DrawLine(orangePen, pivotPoint, coneProjectionPoint);
            //PointF[] transfer = new PointF[correctDistanceTester.Count];
            //correctDistanceTester.CopyTo(transfer);
            //if (transfer.Length >= 2)
            //{
            //    e.Graphics.DrawPolygon(orangePen, transfer);
            //}
            //if (transfer.Length == 1)
            //{
            //    e.Graphics.DrawLine(orangePen, transfer[0], coneProjectionPoint);
            //}
        }

        private void ticks_Tick(object sender, EventArgs e)
        {
            coneProjectionPoint = new PointF(player1.Width + player1.X, (player1.Height / 2) + player1.Y);
            Test.Text = "";
            Test2.Text = "";
            test3.Text = "";
            viewPoints.Clear();
            pointsLinetest.Clear();

            pointGroup1.Clear();
            pointGroup2.Clear();
            pointGroup3.Clear();
            pointGroup4.Clear();
            pointGroup5.Clear();

            visableObjects.Clear();
            visableObjectsMk2.Clear();

            drawnViewPoints.Clear();

            viewConeArray = new PointF[1000];
            viewPointsArray = new PointF[1000];

            lineEnabled.Clear();

            int heightOfCone = 200; //
            int coneCounter = 0;
            while (coneCounter != 101) // add 1 to even split
            {
                pointsLinetest.Add(new PointF(coneProjectionPoint.X + 500, coneProjectionPoint.Y + (heightOfCone)));
                coneCounter++;
                heightOfCone -= 4;
            }
            foreach (PointF i in pointsLinetest)
            {
                lineEnabled.Add(true);
            }

            switchListCounter = 1;
            //int counter9 = 1;
            //while (counter9 <= everyObjectThatMoves.Count)
            //{
            //    pointsSQ.Clear();
            //    pointsSQ.AddRange(everyObjectThatMoves[counter9 - 1]);
            //    ViewPointMaker();
            //    counter9++;
            //}

            correctDistanceTester.Clear();
            int amountInCone = 0;
            bool inCone = false;
            for (int l = 0; l <= everyObjectThatMoves.Count - 1; l++)
            {
                inCone = false;
                for (int j = 0; j <= everyObjectThatMoves[l].Count - 1; j++)
                {
                    List<PointF> temp = new List<PointF>();
                    temp.AddRange(everyObjectThatMoves[l]);
                    if (cone.Contains(temp[j]))
                    {
                        inCone = true;
                        break;
                    }
                }
                if (inCone == true)
                {
                    amountInCone++;
                }
            }

            //List<double> distances = new List<double>();
            List<double> distancesReal = new List<double>();
            List<PointF> distancesTest = new List<PointF>();


            for (int l = 0; l <= amountInCone- 1; l++)
            {
                List<double> distances = new List<double>();
                List<PointF> temp = new List<PointF>();
                temp.AddRange(everyObjectThatMoves[l]);
                for (int j = 0; j <= everyObjectThatMoves[l].Count - 1; j++)
                {
                    if (cone.Contains(temp[j]))
                    {
                        distances.Add(CalculateDistanceFrom(coneProjectionPoint, temp[j]));
                    }
                }
                if (distances.Count > 0)
                {
                    distancesReal.Add(distances.Min());
                    for (int h = 0; h <= everyObjectThatMoves[l].Count - 1; h++)
                    {
                        List<PointF> temp2 = new List<PointF>();
                        temp2.AddRange(everyObjectThatMoves[l]);
                        if (CalculateDistanceFrom(coneProjectionPoint, temp2[h]) == distances.Min())
                        {
                            correctDistanceTester.Add(temp2[h]);
                            visableObjects.Add(everyObjectThatMoves[l]);
                            break;
                        }
                    }
                }
            }
            List<double> tempCopy = new List<double>();
            tempCopy.AddRange(distancesReal);
            int distancesRealCount = distancesReal.Count();
            distancesReal.Sort();
            for (int i = 0; i <= visableObjects.Count - 1; i++)
            {
                List<PointF> temp2 = new List<PointF>();
                temp2.AddRange(visableObjects[i]);
                for (int j = 0; j <= visableObjects[i].Count - 1; j++)
                {
                    if (CalculateDistanceFrom(coneProjectionPoint, temp2[j]) == distancesReal.Min())
                    {
                        distancesReal.Remove(distancesReal.Min());
                        visableObjectsMk2.Add(visableObjects[i]);
                        break;
                    }
                }
            }
            //visableObjectsMk2.Reverse();

            for (int l = 0; l <= visableObjectsMk2.Count - 1; l++)
            {
                pointsSQ.Clear();
                pointsSQ.AddRange(visableObjectsMk2[l]);
                ViewPointMaker();
            }
            //pointsSQ.Clear();
            //pointsSQ.AddRange(pointsSQ1);
            //ViewPointMaker();
            //pointsSQ.Clear();
            //pointsSQ.AddRange(pointsSQ2);
            //ViewPointMaker();
            //pointsSQ.Clear();
            //pointsSQ.AddRange(pointsSQ3);
            //ViewPointMaker();
            int counter = 0; // this is to tell the index of line enabled
            bool falseLineEncountered = false;
            bool firstRunThrough = true;

            #region Rotation Code
            if (qDown == true)
            {
                rotationSpin += rotateSpeed;
            }
            if (eDown == true)
            {
                rotationSpin -= rotateSpeed;
            }
            pivotPoint = new PointF(player1.X + player1.Width / 2, player1.Y + player1.Height / 2);
            int counter2 = 0;
            //List<List<PointF>> lines = new List<List<PointF>>();

            PointF rotatingPoint = new PointF();

            //for (int p = 0; p <= everyObjectThatMoves.Count - 1; p++)
            //{
            //    PointF[] temp = new PointF[everyObjectThatMoves[p].Count];
            //    temp = everyObjectThatMoves[p].ToArray();

            //}
            for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
            {
                PointF[] temp = new PointF[everyObjectThatMoves[i].Count];
                temp = everyObjectThatMoves[i].ToArray();
                counter2 = 0;
                foreach (PointF l in temp)
                {
                    rotatingPoint = RotatePoint(l, pivotPoint, (Math.PI / 180f) * rotationSpin);
                    temp[counter2] = rotatingPoint;
                    counter2++;
                }
                everyObjectThatMoves[i] = temp.ToList();
            }

            //foreach (PointF i in pointsSQ1)
            //{
            //    rotatingPoint = RotatePoint(i, pivotPoint, (Math.PI / 180f) * rotationSpin);
            //    pointsSQ1[counter2] = rotatingPoint;
            //    counter2++;
            //}
            //counter2 = 0;
            //foreach (PointF i in pointsSQ2)
            //{
            //    rotatingPoint = RotatePoint(i, pivotPoint, (Math.PI / 180f) * rotationSpin);
            //    pointsSQ2[counter2] = rotatingPoint;
            //    counter2++;
            //}
            //counter2 = 0;
            //foreach (PointF i in pointsSQ3)
            //{
            //    rotatingPoint = RotatePoint(i, pivotPoint, (Math.PI / 180f) * rotationSpin);
            //    pointsSQ3[counter2] = rotatingPoint;
            //    counter2++;
            //}
            rotationSpin = 0;
            #endregion
            // This code is to rotate everything but the player
            // Q is Left, E is right
            // Still need List of lists that need to be rotated 

            #region EndLineAdder
            foreach (PointF i in pointsLinetest)
            {
                if (lineEnabled[counter] == true)
                {
                    switch (switchListCounter)
                    {
                        case 1:
                            pointGroup1.Add(i);
                            break;
                        case 2:
                            pointGroup2.Add(i);
                            break;
                        case 3:
                            pointGroup3.Add(i);
                            break;
                        case 4:
                            pointGroup4.Add(i);
                            break;
                        case 5:
                            pointGroup5.Add(i);
                            break;
                    }
                    falseLineEncountered = false;
                    firstRunThrough = false;
                }
                else
                {
                    if (firstRunThrough != true)
                    {
                        if (falseLineEncountered == false)
                        {
                            falseLineEncountered = true;
                            switchListCounter++;
                        }
                    }
                }
                counter++;
            }
            #endregion
            // This adds the end point of a line if it doesn't intersect with anything

            List<List<PointF>> pointGroupList = new List<List<PointF>>();
            List<double> averagePoints = new List<double>();
            #region Averager
            if (pointGroup1.Count > 0)
            {
                pointGroup1.Reverse();
                averagePoints.Add(pointGroup1.Average(p => p.Y));
                pointGroupList.Add(pointGroup1);
            }
            if (pointGroup2.Count > 0)
            {
                pointGroup2.Reverse();
                averagePoints.Add(pointGroup2.Average(p => p.Y));
                pointGroupList.Add(pointGroup2);
            }
            if (pointGroup3.Count > 0)
            {
                pointGroup3.Reverse();
                averagePoints.Add(pointGroup3.Average(p => p.Y));
                pointGroupList.Add(pointGroup3);
            }
            if (pointGroup4.Count > 0)
            {
                pointGroup4.Reverse();
                averagePoints.Add(pointGroup4.Average(p => p.Y));
                pointGroupList.Add(pointGroup4);
            }
            if (pointGroup5.Count > 0)
            {
                pointGroup5.Reverse();
                averagePoints.Add(pointGroup5.Average(p => p.Y));
                pointGroupList.Add(pointGroup5);
            }
            if (pointGroup6.Count > 0)
            {
                averagePoints.Add(pointGroup6.Average(p => p.Y));
                pointGroupList.Add(pointGroup6);
            }
            if (pointGroup7.Count > 0)
            {
                averagePoints.Add(pointGroup7.Average(p => p.Y));
                pointGroupList.Add(pointGroup7);
            }
            if (pointGroup8.Count > 0)
            {
                averagePoints.Add(pointGroup8.Average(p => p.Y));
                pointGroupList.Add(pointGroup8);
            }
            averagePoints.Sort((a, b) =>
            {
                int result = a.CompareTo(b);
                return result;
            });
            #endregion
            // This finds the averages of the y points in each points group used
            // This also puts groups used into a list for later use
            // this also orders the averages from smallest to biggest

            #region Debug info printer
            test3.Text += "\n angle: " + rotationAngle;
            foreach (double i in averagePoints)
            {
                test3.Text += "\n point: " + i;
            }
            foreach (PointF i in pointGroup1)
            {
                test3.Text += "\n\n Group1: " + i;
            }
            foreach (PointF i in pointGroup2)
            {
                test3.Text += "\n\n Group2: " + i;
            }
            foreach (PointF i in pointGroup3)
            {
                test3.Text += "\n\n Group3: " + i;
            }
            foreach (PointF i in pointGroup4)
            {
                test3.Text += "\n\n Group4: " + i;
            }
            foreach (PointF i in pointGroup5)
            {
                test3.Text += "\n\n Group5: " + i;
            }

            for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
            {
                List<PointF> temp = everyObjectThatMoves[i];
                for (int p = 0; p <= temp.Count - 1; p++)
                {
                    test3.Text += "\n thing " + temp[p];
                }
            }
            if (amountInCone > 0)
            {
                for (int i = 0; i <= distancesRealCount - 1; i++)
                {
                    List<double> temp = new List<double>();
                    if (tempCopy.Count != 0)
                    {
                        temp.Add(tempCopy[i]);
                    }
                    for (int p = 0; p <= temp.Count - 1; p++)
                    {
                        Test2.Text += "\n distances " + temp[p];
                    }
                }
            }

            Test2.Text += "\n Amount " + amountInCone;
            #endregion
            // This prints out debug info onto a test lable


            foreach (double i in averagePoints)
            {
                int up = 0;
                foreach (List<PointF> j in pointGroupList)
                {
                    if (j.Average(p => p.Y) == i)
                    {
                        foreach (PointF k in j)
                        {
                            drawnViewPoints.Add(k);
                        }
                        pointsLinetest.RemoveAt(up);
                    }
                    up++;
                }
            }

            foreach (Boolean i in lineEnabled)
            {
                Test.Text += "\n\n Line " + i;
            }
            //foreach (PointF i in pointsSQ)
            //{
            //    bool numberIsInPolygonCone = false;
            //    numberIsInPolygonCone = IsInPolygon(pointsLinetest.ToArray(), i);
            //    if (numberIsInPolygonCone == true)
            //    {
            //        //Test.Text += "\n"+ i;
            //        viewPoints.Add(i);
            //    }
            //}
            //viewPoints.Sort((a, b) =>
            //{
            //    int result = a.Y.CompareTo(b.Y);
            //    if (result == 0) result = a.Y.CompareTo(b.Y);
            //    return result;
            //});
            //viewPoints.Insert(0, coneProjectionPoint);
            //sort all seperate lists by x and y after  new lists are created and then add them together to draw the points
            //int up = 1;
            //List<double> distancesOfDrawPoints = new List<double>();
            //foreach (PointF i in viewPoints)
            //{
            //    if (up == viewPoints.Count())
            //    {
            //        up = viewPoints.Count() - 2;
            //    }
            //    distancesOfDrawPoints.Add(CalculateDistanceFrom(i, viewPoints[up]));
            //    up++;
            //}

           
            drawnViewPoints.Add(coneProjectionPoint);
            foreach (PointF i in drawnViewPoints)
            {
                Test2.Text += "\n\n" + i;
            }

            // This is done because certain operations are easier to do on arrays rather than lists
            drawnViewPoints.CopyTo(viewPointsArray);
            pointsLinetest.CopyTo(viewConeArray);

            int zeroCounter = 0;

            foreach (PointF i in viewPointsArray)
            {
                if (i.X == 0 && i.Y == 0)
                {
                    viewPointsArray[zeroCounter] = coneProjectionPoint;
                }
                zeroCounter++;
            }

            Test.Text += "\n" + drawnViewPoints.Count;

            // All code bellow is to move the objects on screen around the character
            if (wDown == true)
            {
                for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
                {
                    PointF[] temp = new PointF[everyObjectThatMoves[i].Count];
                    List<PointF> temp2 = new List<PointF>();
                    everyObjectThatMoves[i].CopyTo(temp);
                    int counter4 = 0;
                    foreach (PointF j in temp)
                    {
                        temp[counter4] = new PointF(j.X, j.Y + speed);
                        counter4++;
                    }
                    foreach (PointF l in temp)
                    {
                        temp2.Add(new PointF(l.X, l.Y));
                    }
                    everyObjectThatMoves[i] = temp2;
                }



                // need to add these all to a list of lists of points so it's more efficient to add more game objects
                //int counter3 = 0;
                //foreach (PointF i in pointsSQ1)
                //{
                //    pointsSQ1[counter3] = new PointF(i.X, i.Y + speed);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ2)
                //{
                //    pointsSQ2[counter3] = new PointF(i.X, i.Y + speed);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ3)
                //{
                //    pointsSQ3[counter3] = new PointF(i.X, i.Y + speed);
                //    counter3++;
                //}
            }
            if (sDown == true)
            {
                for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
                {
                    PointF[] temp = new PointF[everyObjectThatMoves[i].Count];
                    List<PointF> temp2 = new List<PointF>();
                    everyObjectThatMoves[i].CopyTo(temp);
                    int counter4 = 0;
                    foreach (PointF j in temp)
                    {
                        temp[counter4] = new PointF(j.X, j.Y - speed);
                        counter4++;
                    }
                    foreach (PointF l in temp)
                    {
                        temp2.Add(new PointF(l.X, l.Y));
                    }
                    everyObjectThatMoves[i] = temp2;
                }

                //int counter3 = 0;
                //foreach (PointF i in pointsSQ1)
                //{
                //    pointsSQ1[counter3] = new PointF(i.X, i.Y - speed);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ2)
                //{
                //    pointsSQ2[counter3] = new PointF(i.X, i.Y - speed);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ3)
                //{
                //    pointsSQ3[counter3] = new PointF(i.X, i.Y - speed);
                //    counter3++;
                //}
            }
            if (aDown == true)
            {
                for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
                {
                    PointF[] temp = new PointF[everyObjectThatMoves[i].Count];
                    List<PointF> temp2 = new List<PointF>();
                    everyObjectThatMoves[i].CopyTo(temp);
                    int counter4 = 0;
                    foreach (PointF j in temp)
                    {
                        temp[counter4] = new PointF(j.X + speed, j.Y);
                        counter4++;
                    }
                    foreach (PointF l in temp)
                    {
                        temp2.Add(new PointF(l.X, l.Y));
                    }
                    everyObjectThatMoves[i] = temp2;
                }

                //int counter3 = 0;
                //foreach (PointF i in pointsSQ1)
                //{
                //    pointsSQ1[counter3] = new PointF(i.X + speed, i.Y);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ2)
                //{
                //    pointsSQ2[counter3] = new PointF(i.X + speed, i.Y);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ3)
                //{
                //    pointsSQ3[counter3] = new PointF(i.X + speed, i.Y);
                //    counter3++;
                //}
            }
            if (dDown == true)
            {
                for (int i = 0; i <= everyObjectThatMoves.Count - 1; i++)
                {
                    PointF[] temp = new PointF[everyObjectThatMoves[i].Count];
                    List<PointF> temp2 = new List<PointF>();
                    everyObjectThatMoves[i].CopyTo(temp);
                    int counter4 = 0;
                    foreach (PointF j in temp)
                    {
                        temp[counter4] = new PointF(j.X - speed, j.Y);
                        counter4++;
                    }
                    foreach (PointF l in temp)
                    {
                        temp2.Add(new PointF(l.X, l.Y));
                    }
                    everyObjectThatMoves[i] = temp2;
                }


                //int counter3 = 0;
                //foreach (PointF i in pointsSQ1)
                //{
                //    pointsSQ1[counter3] = new PointF(i.X - speed, i.Y);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ2)
                //{
                //    pointsSQ2[counter3] = new PointF(i.X - speed, i.Y);
                //    counter3++;
                //}
                //counter3 = 0;
                //foreach (PointF i in pointsSQ3)
                //{
                //    pointsSQ3[counter3] = new PointF(i.X - speed, i.Y);
                //    counter3++;
                //}
            }
            Refresh();
        }

        //void Obstical()
        //{
        //    Graphics g = this.CreateGraphics();
        //    PointF[] temp = new PointF[3];
        //    temp = pointsSQ1.ToArray();
        //    g.DrawLines(greenPen, temp);
        //}

        //void Obstical2()
        //{
        //    Graphics g = this.CreateGraphics();
        //    PointF[] temp = new PointF[3];
        //    temp = pointsSQ2.ToArray();
        //    g.DrawLines(greenPen, temp);
        //}

        //void Obstical3()
        //{
        //    Graphics g = this.CreateGraphics();
        //    PointF[] temp = new PointF[3];
        //    temp = pointsSQ3.ToArray();
        //    g.DrawLines(greenPen, temp);
        //}

        void lineLineIntersection(PointF A, PointF B, PointF C, PointF D)
        {
            // Line AB represented as a1x + b1y = c1
            double a1 = B.Y - A.Y;
            double b1 = A.X - B.X;
            double c1 = a1 * (A.X) + b1 * (A.Y);

            // Line CD represented as a2x + b2y = c2
            double a2 = D.Y - C.Y;
            double b2 = C.X - D.X;
            double c2 = a2 * (C.X) + b2 * (C.Y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                // The lines are parallel. This is simplified
                // by returning a pair of FLT_MAX
                intersectionPoint = new PointF(0, 0);
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                intersectionPoint = new PointF(Convert.ToInt32(x), Convert.ToInt32(y));
            }
        }

        ////////////////////////////////////////////////////////////////////////test code

        //Given three collinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        static Boolean onSegment(PointF p, PointF q, PointF r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        static int orientation(PointF p, PointF q, PointF r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            int val = Convert.ToInt32((q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y));

            if (val == 0) return 0; // collinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        static Boolean doIntersect(PointF p1, PointF q1, PointF p2, PointF q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        public static bool IsInPolygon(PointF[] poly, PointF p)
        {
            PointF p1, p2;
            bool inside = false;

            if (poly.Length < 3)
            {
                return inside;
            }

            var oldPoint = new PointF(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new PointF(poly[i].X, poly[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }


        public void ViewPointMaker()
        {
            PointF coneProjectionPoint = new PointF(player1.Width + player1.X, (player1.Height / 2) + player1.Y);
            int lineCounter = 0; // This is to indicate at which line the line enabler should or shouldn't make a line enabled
            int lineEnabledCounter = 0; // This is uses to indicate which line the line enabler is effect
            bool switchListCounterIncreasable = false;
            foreach (PointF i in pointsLinetest)
            {
                int obsticlePointCounter = 0; // This is used to indicate at where in pointsSQ (name pending) is the first point of the line in the index
                int nextObsticlePointCounter = 1; // This is used to indicate at where in pointsSQ (name pending) is the next point of the line in the index 
                int runThroughSQ = 1; // This is used to run through all points that are in pointsSQ, the obstical
                bool intersects = false; // If one of the lines in pointsLineTest intersects with a line running from obsticlePointCounter to nextObsticlePointCounter in pointsSQ, this becomes true
                List<PointF> pointsOfIntersection = new List<PointF>(); //If a line from pointsLineTest intersects lines from pointsSQ multiple times, multiple points are added here
                List<double> distances = new List<double>(); // List of distances from points in pointsOfIntersection and coneProjectionPoint
                if (lineEnabled[lineEnabledCounter] == true) // If the line from pointsLineTest is enabled, it will be checked for intersection with an obsticle (pointsSQ)
                {
                    while (runThroughSQ <= pointsSQ.Count)
                    {
                        if (nextObsticlePointCounter == pointsSQ.Count)
                        {
                            nextObsticlePointCounter = obsticlePointCounter - 1;
                            // This code is to check if nextObsticlePointCounter is out of the scope of the list
                            // If so this would mean that it's checking a non existant point
                            // this code then makes the next code check obsticlePointCounter to the previous point as a line for interception
                        }
                        PointF p1 = new PointF(coneProjectionPoint.X, coneProjectionPoint.Y);
                        PointF q1 = new PointF(i.X, i.Y);
                        PointF p2 = new PointF(pointsSQ[obsticlePointCounter].X, pointsSQ[obsticlePointCounter].Y);
                        PointF q2 = new PointF(pointsSQ[nextObsticlePointCounter].X, pointsSQ[nextObsticlePointCounter].Y);
                        intersects = doIntersect(p1, q1, p2, q2);
                        if (intersects == true)
                        {
                            
                            PointF A = new PointF(coneProjectionPoint.X, coneProjectionPoint.Y);
                            PointF B = new PointF(i.X, i.Y);
                            PointF C = new PointF(pointsSQ[obsticlePointCounter].X, pointsSQ[obsticlePointCounter].Y);
                            PointF D = new PointF(pointsSQ[nextObsticlePointCounter].X, pointsSQ[nextObsticlePointCounter].Y);
                            lineLineIntersection(A, B, C, D);
                            pointsOfIntersection.Add(intersectionPoint);
                            lineEnabled.RemoveAt(lineCounter);
                            lineEnabled.Insert(lineCounter, false);
                        }
                        runThroughSQ++;
                        obsticlePointCounter++;
                        nextObsticlePointCounter = obsticlePointCounter + 1;
                    }
                    if (pointsOfIntersection.Count() == 0)
                    {
                        lineEnabled.RemoveAt(lineCounter);
                        lineEnabled.Insert(lineCounter, true);
                    }
                    //else if(pointsOfIntersection.Count() == 1)
                    //{
                    //    switch (switchListCounter)
                    //    {
                    //        case 1:
                    //            pointGroup1.Add(pointsOfIntersection[0]);
                    //            break;
                    //        case 2:
                    //            pointGroup2.Add(pointsOfIntersection[0]);
                    //            break;
                    //        case 3:
                    //            pointGroup3.Add(pointsOfIntersection[0]);
                    //            break;
                    //        case 4:
                    //            pointGroup4.Add(pointsOfIntersection[0]);
                    //            break;
                    //        case 5:
                    //            pointGroup5.Add(pointsOfIntersection[0]);
                    //            break;
                    //    }
                    //    switchListCounterIncreasable = true;
                    //}
                    else
                    {
                        foreach (PointF l in pointsOfIntersection)
                        {
                            distances.Add(CalculateDistanceFrom(l, coneProjectionPoint));
                        }
                        distances.Sort((a, b) =>
                        {
                            int result = a.CompareTo(b);
                            return result;
                        });
                        foreach (PointF j in pointsOfIntersection)
                        {
                            double transfer = CalculateDistanceFrom(j, coneProjectionPoint);
                            if (transfer == distances[0])
                            {
                                switch (switchListCounter)
                                {
                                    case 1:
                                        pointGroup1.Add(j);
                                        break;
                                    case 2:
                                        pointGroup2.Add(j);
                                        break;
                                    case 3:
                                        pointGroup3.Add(j);
                                        break;
                                    case 4:
                                        pointGroup4.Add(j);
                                        break;
                                    case 5:
                                        pointGroup5.Add(j);
                                        break;
                                }
                                switchListCounterIncreasable = true;
                                break;
                            }
                        }
                    }
                    lineCounter++;
                    lineEnabledCounter++;
                }
            }
            if (switchListCounterIncreasable == true)
            {
                switchListCounter++;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Double CalculateDistanceFrom(PointF point, PointF target)
        {
            //a^2 + b^2 = c^2
            //or c = sqaure root of a^2 + b^2
            double a = target.X - point.X;
            double b = target.Y - point.Y;
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }
        ////////////////////////////////////////////////////////////////
        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The center point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        private PointF RotatePoint(PointF point, PointF pivot, double radians)
        {
            var cosTheta = Math.Cos(radians);
            var sinTheta = Math.Sin(radians);

            var x = (cosTheta * (point.X - pivot.X) - sinTheta * (point.Y - pivot.Y) + pivot.X);
            var y = (sinTheta * (point.X - pivot.X) + cosTheta * (point.Y - pivot.Y) + pivot.Y);

            return new PointF((float)x, (float)y);
        }
    }
}

