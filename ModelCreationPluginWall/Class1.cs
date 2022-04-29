using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreationPluginWall
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> listLevel = GetLevels(doc);

            double width = UnitUtils.ConvertToInternalUnits(10000, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(5000, UnitTypeId.Millimeters);

            List<XYZ> points = SetPoints(width, depth);

            List<Wall> walls = CreateWalls(doc, listLevel, points);

            return Result.Succeeded;
        }

        private List<Level> GetLevels(Document doc)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();
            return listLevel;
        }

        public List<XYZ> SetPoints(double width, double depth)
        {
            double dx = width / 2;
            double dy = depth / 2;
            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));
            return (points);
        }

        public List<Wall> CreateWalls(Document doc, List<Level> listLevel, List<XYZ> points)
        {
            var level1 = listLevel
                .Where(x => x.Name.Equals("Уровень 1"))
                .FirstOrDefault();

            var level2 = listLevel
                .Where(x => x.Name.Equals("Уровень 2"))
                .FirstOrDefault();

            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Создание стен");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            //AddDoor(doc, level1, walls[0]);
            //AddWindow1(doc, level1, walls[1]);
            //AddWindow2(doc, level1, walls[2]);
            //AddWindow3(doc, level1, walls[3]);
            //AddRoof(doc, level2, walls);
            transaction.Commit();

            return (walls);
        }


        //private void AddDoor(Document doc, Level level1, Wall wall)
        //{
        //    FamilySymbol doorType = new FilteredElementCollector(doc)
        //        .OfClass(typeof(FamilySymbol))
        //        .OfCategory(BuiltInCategory.OST_Doors)
        //        .OfType<FamilySymbol>()
        //        .Where(x => x.Name.Equals("0915 x 2134 мм"))
        //        .Where(x => x.FamilyName.Equals("Одиночные-Щитовые"))
        //        .FirstOrDefault();

        //    LocationCurve hostCurve = wall.Location as LocationCurve;
        //    XYZ point1 = hostCurve.Curve.GetEndPoint(0);
        //    XYZ point2 = hostCurve.Curve.GetEndPoint(1);
        //    XYZ point = (point1 + point2) / 2;

        //    if (!doorType.IsActive)
        //    {
        //        doorType.Activate();
        //    }

        //    doc.Create.NewFamilyInstance(point, doorType, wall, level1, StructuralType.NonStructural);

        //}

        //private void AddWindow1(Document doc, Level level1, Wall wall)
        //{
        //    FamilySymbol windowType = new FilteredElementCollector(doc)
        //        .OfClass(typeof(FamilySymbol))
        //        .OfCategory(BuiltInCategory.OST_Windows)
        //        .OfType<FamilySymbol>()
        //        .Where(x => x.Name.Equals("0406 x 1220 мм"))
        //        .Where(x => x.FamilyName.Equals("Фиксированные"))
        //        .FirstOrDefault();

        //    LocationCurve hostCurve = wall.Location as LocationCurve;
        //    XYZ point1 = hostCurve.Curve.GetEndPoint(0);
        //    XYZ point2 = hostCurve.Curve.GetEndPoint(1);
        //    XYZ point = (point1 + point2) / 2;

        //    if (!windowType.IsActive)
        //    {
        //        windowType.Activate();
        //    }


        //    FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);
        //    Parameter offset = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
        //    offset.Set(UnitUtils.ConvertToInternalUnits(1000, UnitTypeId.Millimeters));

        //}

        //private void AddWindow2(Document doc, Level level1, Wall wall)
        //{
        //    FamilySymbol windowType = new FilteredElementCollector(doc)
        //        .OfClass(typeof(FamilySymbol))
        //        .OfCategory(BuiltInCategory.OST_Windows)
        //        .OfType<FamilySymbol>()
        //        .Where(x => x.Name.Equals("0915 x 1830 мм"))
        //        .Where(x => x.FamilyName.Equals("Фиксированные"))
        //        .FirstOrDefault();

        //    LocationCurve hostCurve = wall.Location as LocationCurve;
        //    XYZ point1 = hostCurve.Curve.GetEndPoint(0);
        //    XYZ point2 = hostCurve.Curve.GetEndPoint(1);
        //    XYZ point = (point1 + point2) / 2;

        //    if (!windowType.IsActive)
        //    {
        //        windowType.Activate();
        //    }


        //    FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);
        //    Parameter offset = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
        //    offset.Set(UnitUtils.ConvertToInternalUnits(800, UnitTypeId.Millimeters));
        //}

        //private void AddWindow3(Document doc, Level level1, Wall wall)
        //{
        //    FamilySymbol windowType = new FilteredElementCollector(doc)
        //        .OfClass(typeof(FamilySymbol))
        //        .OfCategory(BuiltInCategory.OST_Windows)
        //        .OfType<FamilySymbol>()
        //        .Where(x => x.Name.Equals("0915 x 0610 мм"))
        //        .Where(x => x.FamilyName.Equals("Фиксированные"))
        //        .FirstOrDefault();

        //    LocationCurve hostCurve = wall.Location as LocationCurve;
        //    XYZ point1 = hostCurve.Curve.GetEndPoint(0);
        //    XYZ point2 = hostCurve.Curve.GetEndPoint(1);
        //    XYZ point = (point1 + point2) / 2;

        //    if (!windowType.IsActive)
        //    {
        //        windowType.Activate();
        //    }


        //    FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);
        //    Parameter offset = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
        //    offset.Set(UnitUtils.ConvertToInternalUnits(1000, UnitTypeId.Millimeters));
        //}
    }
}
