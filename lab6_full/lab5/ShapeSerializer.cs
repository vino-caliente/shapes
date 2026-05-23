using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.Text;

namespace lab1
{
    public class ShapeSerializer
    {
        [DataContract]
        private class ShapeData
        {
            [DataMember] public string Type { get; set; }
            [DataMember] public int X { get; set; }
            [DataMember] public int Y { get; set; }
            [DataMember] public int ColorArgb { get; set; }
            [DataMember] public int LineWidth { get; set; }
            [DataMember] public bool IsFilled { get; set; }
            [DataMember] public int Width { get; set; }
            [DataMember] public int Height { get; set; }
            [DataMember] public int Radius { get; set; }
            [DataMember] public int EndX { get; set; }
            [DataMember] public int EndY { get; set; }
            [DataMember] public int Point2X { get; set; }
            [DataMember] public int Point2Y { get; set; }
            [DataMember] public int Point3X { get; set; }
            [DataMember] public int Point3Y { get; set; }
            [DataMember] public int OuterRadius { get; set; }
            [DataMember] public int InnerRadius { get; set; }
            [DataMember] public int Points { get; set; }
        }

        public string Serialize(ShapeList list)
        {
            var dataList = new List<ShapeData>();

            foreach (var shape in list.GetAll())
            {
                var data = new ShapeData();

                if (shape is Triangle t)
                {
                    data.Type = "Triangle";
                    data.X = t.X;
                    data.Y = t.Y;
                    data.Point2X = t.Point2.X;
                    data.Point2Y = t.Point2.Y;
                    data.Point3X = t.Point3.X;
                    data.Point3Y = t.Point3.Y;
                    data.IsFilled = t.IsFilled;
                }
                else if (shape is Rectangle r)
                {
                    data.Type = "Rectangle";
                    data.X = r.X;
                    data.Y = r.Y;
                    data.Width = r.Width;
                    data.Height = r.Height;
                    data.IsFilled = r.IsFilled;
                }
                else if (shape is Square s)
                {
                    data.Type = "Square";
                    data.X = s.X;
                    data.Y = s.Y;
                    data.Width = s.Side;
                    data.Height = s.Side;
                    data.IsFilled = s.IsFilled;
                }
                else if (shape is Ellipse e)
                {
                    data.Type = "Ellipse";
                    data.X = e.X;
                    data.Y = e.Y;
                    data.Width = e.Width;
                    data.Height = e.Height;
                    data.IsFilled = e.IsFilled;
                }
                else if (shape is Circle c)
                {
                    data.Type = "Circle";
                    data.X = c.X;
                    data.Y = c.Y;
                    data.Radius = c.Radius;
                    data.IsFilled = c.IsFilled;
                }
                else if (shape is Line l)
                {
                    data.Type = "Line";
                    data.X = l.X;
                    data.Y = l.Y;
                    data.EndX = l.EndX;
                    data.EndY = l.EndY;
                }
                // ← ИСПРАВЛЕНО: проверяем по имени типа
                else if (shape.GetType().Name == "Star")
                {
                    dynamic star = shape;
                    data.Type = "Star";
                    data.X = star.X;
                    data.Y = star.Y;
                    data.OuterRadius = star.OuterRadius;
                    data.InnerRadius = star.InnerRadius;
                    data.Points = star.Points;
                    data.IsFilled = star.IsFilled;
                    data.LineWidth = star.LineWidth;
                    data.ColorArgb = star.Color.ToArgb();
                }
                else
                {
                    // Неизвестный тип — пропускаем
                    System.Diagnostics.Debug.WriteLine($"Unknown shape type: {shape.GetType().Name}");
                    continue;
                }

                data.ColorArgb = shape.Color.ToArgb();
                data.LineWidth = shape.LineWidth;
                dataList.Add(data);
            }

            var serializer = new DataContractJsonSerializer(typeof(List<ShapeData>));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, dataList);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public ShapeList Deserialize(string json, PluginManager shapePluginManager)
        {
            var list = new ShapeList();
            var serializer = new DataContractJsonSerializer(typeof(List<ShapeData>));

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var dataList = (List<ShapeData>)serializer.ReadObject(stream);

                foreach (var data in dataList)
                {
                    Color color = Color.FromArgb(data.ColorArgb);

                    switch (data.Type)
                    {
                        case "Triangle":
                            list.Add(new Triangle(new Point(data.X, data.Y), new Point(data.Point2X, data.Point2Y), new Point(data.Point3X, data.Point3Y), color, data.IsFilled, data.LineWidth));
                            break;
                        case "Rectangle":
                            list.Add(new Rectangle(data.X, data.Y, data.Width, data.Height, color, data.IsFilled, data.LineWidth));
                            break;
                        case "Square":
                            list.Add(new Square(data.X, data.Y, data.Width, color, data.IsFilled, data.LineWidth));
                            break;
                        case "Ellipse":
                            list.Add(new Ellipse(data.X, data.Y, data.Width, data.Height, color, data.IsFilled, data.LineWidth));
                            break;
                        case "Circle":
                            list.Add(new Circle(data.X, data.Y, data.Radius, color, data.IsFilled, data.LineWidth));
                            break;
                        case "Line":
                            list.Add(new Line(data.X, data.Y, data.EndX, data.EndY, color, data.LineWidth));
                            break;
                        case "Star":
                            var starPlugin = shapePluginManager?.GetPlugins().Find(p => p.ShapeName == "Star");
                            if (starPlugin != null)
                            {
                                var starType = starPlugin.GetShapeType();
                                var star = Activator.CreateInstance(starType,
                                    data.X, data.Y, data.OuterRadius, data.InnerRadius, data.Points, color, data.IsFilled, data.LineWidth);
                                list.Add((Shape)star);
                            }
                            else
                            {
                                // Fallback: загружаем через Assembly (но так не делать, если можно)
                                var starAssembly = Assembly.Load("StarPlugin");
                                var starType = starAssembly.GetType("StarPlugin.Star");
                                var star = Activator.CreateInstance(starType,
                                    data.X, data.Y, data.OuterRadius, data.InnerRadius, data.Points, color, data.IsFilled, data.LineWidth);
                                list.Add((Shape)star);
                            }
                            break;
                    }
                }
            }

            return list;
        }
    }
}