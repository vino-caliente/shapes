using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TransportSerialization
{
    // Базовый абстрактный класс
    // Описываются типы всех возможных сериализуемых объектов
    [Serializable]
    [XmlInclude(typeof(Car))]
    [XmlInclude(typeof(Truck))]
    [XmlInclude(typeof(Motorcycle))]
    [XmlInclude(typeof(ElectricCar))]
    [XmlInclude(typeof(Bicycle))]
    public abstract class Vehicle
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        protected Vehicle()
        {
            Brand = string.Empty;
            Model = string.Empty;
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Brand} {Model} ({Year})";
        }
    }

    // Класс Car (автомобиль)
    public class Car : Vehicle
    {
        public int DoorsCount { get; set; }

        public Car() : base() { }
    }

    // Класс Truck (грузовик)
    public class Truck : Vehicle
    {
        public double LoadCapacity { get; set; }

        public Truck() : base() { }
    }

    // Класс Motorcycle (мотоцикл)
    public class Motorcycle : Vehicle
    {
        public bool HasSidecar { get; set; }

        public Motorcycle() : base() { }
    }

    // Класс ElectricCar (электромобиль)
    public class ElectricCar : Car
    {
        public double BatteryCapacity { get; set; }

        public ElectricCar() : base() { }
    }

    // Класс Bicycle (велосипед)
    public class Bicycle : Vehicle
    {
        public int GearsCount { get; set; }

        public Bicycle() : base() { }
    }

    // Основной класс программы
    class Program
    {
        private static List<Vehicle> vehicles = new List<Vehicle>();
        private static readonly string filePath = "vehicles.xml";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления транспортом ===\n");

            // Добавляем тестовые данные
            InitializeTestData();

            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowVehicles();
                        break;
                    case "2":
                        AddVehicle();
                        break;
                    case "3":
                        EditVehicle();
                        break;
                    case "4":
                        RemoveVehicle();
                        break;
                    case "5":
                        SerializeToXml();
                        break;
                    case "6":
                        DeserializeFromXml();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void InitializeTestData()
        {
            vehicles.Add(new Car { Brand = "Toyota", Model = "Camry", Year = 2020, DoorsCount = 4 });
            vehicles.Add(new Truck { Brand = "Volvo", Model = "FH16", Year = 2019, LoadCapacity = 20.5 });
            vehicles.Add(new Motorcycle { Brand = "Harley-Davidson", Model = "Street 750", Year = 2021, HasSidecar = false });
            vehicles.Add(new ElectricCar { Brand = "Tesla", Model = "Model 3", Year = 2022, DoorsCount = 4, BatteryCapacity = 75.0 });
            vehicles.Add(new Bicycle { Brand = "Giant", Model = "Escape 3", Year = 2023, GearsCount = 21 });
        }

        static void ShowMenu()
        {
            Console.WriteLine("1. Показать все транспортные средства");
            Console.WriteLine("2. Добавить транспортное средство");
            Console.WriteLine("3. Редактировать транспортное средство");
            Console.WriteLine("4. Удалить транспортное средство");
            Console.WriteLine("5. Сохранить в XML");
            Console.WriteLine("6. Загрузить из XML");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        static void ShowVehicles()
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("\nСписок пуст.");
                return;
            }

            Console.WriteLine("\nСписок транспортных средств:");
            for (int i = 0; i < vehicles.Count; i++)
            {
                Console.WriteLine($"[{i}] {vehicles[i]}");
                DisplayDetails(vehicles[i]);
            }
        }

        static void DisplayDetails(Vehicle vehicle)
        {
            if (vehicle is Car car && !(vehicle is ElectricCar))
            {
                Console.WriteLine($"    Количество дверей: {car.DoorsCount}");
            }
            else if (vehicle is Truck truck)
            {
                Console.WriteLine($"    Грузоподъемность: {truck.LoadCapacity} т");
            }
            else if (vehicle is Motorcycle motorcycle)
            {
                Console.WriteLine($"    Коляска: {(motorcycle.HasSidecar ? "Да" : "Нет")}");
            }
            else if (vehicle is ElectricCar electricCar)
            {
                Console.WriteLine($"    Количество дверей: {electricCar.DoorsCount}");
                Console.WriteLine($"    Емкость батареи: {electricCar.BatteryCapacity} кВт·ч");
            }
            else if (vehicle is Bicycle bicycle)
            {
                Console.WriteLine($"    Количество скоростей: {bicycle.GearsCount}");
            }
        }

        static void AddVehicle()
        {
            Console.WriteLine("\nВыберите тип транспортного средства:");
            Console.WriteLine("1. Автомобиль (Car)");
            Console.WriteLine("2. Грузовик (Truck)");
            Console.WriteLine("3. Мотоцикл (Motorcycle)");
            Console.WriteLine("4. Электромобиль (ElectricCar)");
            Console.WriteLine("5. Велосипед (Bicycle)");
            Console.Write("Ваш выбор: ");

            string typeChoice = Console.ReadLine();

            try
            {
                Vehicle newVehicle = CreateVehicleByType(typeChoice);
                if (newVehicle != null)
                {
                    vehicles.Add(newVehicle);
                    Console.WriteLine("Транспортное средство успешно добавлено!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static Vehicle CreateVehicleByType(string typeChoice)
        {
            Console.Write("Введите бренд: ");
            string brand = Console.ReadLine();

            Console.Write("Введите модель: ");
            string model = Console.ReadLine();

            Console.Write("Введите год выпуска: ");
            int year = int.Parse(Console.ReadLine());

            switch (typeChoice)
            {
                case "1":
                    Console.Write("Введите количество дверей: ");
                    int doors = int.Parse(Console.ReadLine());
                    return new Car { Brand = brand, Model = model, Year = year, DoorsCount = doors };

                case "2":
                    Console.Write("Введите грузоподъемность (тонн): ");
                    double capacity = double.Parse(Console.ReadLine());
                    return new Truck { Brand = brand, Model = model, Year = year, LoadCapacity = capacity };

                case "3":
                    Console.Write("Наличие коляски (true/false): ");
                    bool hasSidecar = bool.Parse(Console.ReadLine());
                    return new Motorcycle { Brand = brand, Model = model, Year = year, HasSidecar = hasSidecar };

                case "4":
                    Console.Write("Введите количество дверей: ");
                    int electricDoors = int.Parse(Console.ReadLine());
                    Console.Write("Введите емкость батареи (кВт·ч): ");
                    double battery = double.Parse(Console.ReadLine());
                    return new ElectricCar { Brand = brand, Model = model, Year = year, DoorsCount = electricDoors, BatteryCapacity = battery };

                case "5":
                    Console.Write("Введите количество скоростей: ");
                    int gears = int.Parse(Console.ReadLine());
                    return new Bicycle { Brand = brand, Model = model, Year = year, GearsCount = gears };

                default:
                    Console.WriteLine("Неверный тип.");
                    return null;
            }
        }

        static void EditVehicle()
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("\nСписок пуст. Нечего редактировать.");
                return;
            }

            ShowVehicles();
            Console.Write("\nВведите индекс для редактирования: ");

            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < vehicles.Count)
            {
                Vehicle oldVehicle = vehicles[index];
                Console.WriteLine($"\nРедактирование: {oldVehicle}");

                try
                {
                    Vehicle editedVehicle = CreateVehicleByType(GetVehicleTypeChoice(oldVehicle));
                    if (editedVehicle != null)
                    {
                        vehicles[index] = editedVehicle;
                        Console.WriteLine("Транспортное средство успешно обновлено!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при редактировании: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Неверный индекс.");
            }
        }

        static string GetVehicleTypeChoice(Vehicle vehicle)
        {
            if (vehicle is ElectricCar) return "4";
            if (vehicle is Car) return "1";
            if (vehicle is Truck) return "2";
            if (vehicle is Motorcycle) return "3";
            if (vehicle is Bicycle) return "5";
            return "1";
        }

        static void RemoveVehicle()
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("\nСписок пуст. Нечего удалять.");
                return;
            }

            ShowVehicles();
            Console.Write("\nВведите индекс для удаления: ");

            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < vehicles.Count)
            {
                vehicles.RemoveAt(index);
                Console.WriteLine("Транспортное средство удалено!");
            }
            else
            {
                Console.WriteLine("Неверный индекс.");
            }
        }

        static void SerializeToXml()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Vehicle>), new Type[] {
                    typeof(Car), typeof(Truck), typeof(Motorcycle),
                    typeof(ElectricCar), typeof(Bicycle)
                });

                using (TextWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, vehicles);
                }

                Console.WriteLine($"Данные сохранены в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }
        }

        static void DeserializeFromXml()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Файл {filePath} не найден.");
                    return;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Vehicle>), new Type[] {
                    typeof(Car), typeof(Truck), typeof(Motorcycle),
                    typeof(ElectricCar), typeof(Bicycle)
                });

                using (TextReader reader = new StreamReader(filePath))
                {
                    vehicles = (List<Vehicle>)serializer.Deserialize(reader);
                }

                Console.WriteLine($"Данные загружены из файла: {filePath}");
                Console.WriteLine($"Загружено объектов: {vehicles.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
            }
        }
    }
}