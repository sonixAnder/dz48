using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RangeCheckAttribute : Attribute
{
    public int Min { get; }
    public int Max { get; }

    public RangeCheckAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class StringLengthCheckAttribute : Attribute
{
    public int MaxLength { get; }

    public StringLengthCheckAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }
}

public class UserProfile
{
    [RangeCheck(18, 120)]  // Проверка возраста: от 18 до 120 лет
    public int Age { get; set; }

    [StringLengthCheck(50)]  // Проверка имени: не более 50 символов
    public string Name { get; set; }

    [StringLengthCheck(100)]  // Проверка адреса: не более 100 символов
    public string Address { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var userProfile = new UserProfile
        {
            Age = 25,
            Name = "Иван Иванов",
            Address = "ул. Пушкина, дом 10, квартира 5"
        };

        var validationResult = ValidateObject(userProfile);
        if (validationResult)
        {
            Console.WriteLine("Все данные пользователя валидны.");
        }
        else
        {
            Console.WriteLine("Ошибка валидации данных пользователя.");
        }
    }

    public static bool ValidateObject(object obj)
    {
        bool isValid = true;

        var properties = obj.GetType().GetProperties();
        foreach (var property in properties)
        {
            var rangeAttribute = (RangeCheckAttribute)Attribute.GetCustomAttribute(property, typeof(RangeCheckAttribute));
            if (rangeAttribute != null)
            {
                int propertyValue = (int)property.GetValue(obj);
                if (propertyValue < rangeAttribute.Min || propertyValue > rangeAttribute.Max)
                {
                    Console.WriteLine($"Ошибка: {property.Name} должно быть в пределах от {rangeAttribute.Min} до {rangeAttribute.Max}. Текущее значение: {propertyValue}");
                    isValid = false;
                }
            }

            var stringLengthAttribute = (StringLengthCheckAttribute)Attribute.GetCustomAttribute(property, typeof(StringLengthCheckAttribute));
            if (stringLengthAttribute != null)
            {
                string propertyValue = (string)property.GetValue(obj);
                if (string.IsNullOrEmpty(propertyValue))
                {
                    Console.WriteLine($"Ошибка: {property.Name} не может быть пустым.");
                    isValid = false;
                }
                else if (propertyValue.Length > stringLengthAttribute.MaxLength)
                {
                    Console.WriteLine($"Ошибка: {property.Name} не может быть длиннее {stringLengthAttribute.MaxLength} символов. Текущее значение: {propertyValue}");
                    isValid = false;
                }
            }
        }
        return isValid;
    }
}