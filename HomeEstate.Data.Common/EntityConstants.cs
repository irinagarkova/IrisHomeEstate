namespace HomeEstate.HomeEstateCommon
{
    public class EntityConstants
    {
        public static class PropertyConstants
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 100;
            public const string TitleRequiredErrorMessage = "Title is required!";

            public const int DescriptionMinLength = 20;
            public const int DescriptionMaxLength = 1000;
            public const string DescriptionRequiredErrorMessage = "Description is required!";

            public const decimal PriceMinValue = 1m;
            public const decimal PriceMaxValue = 1_000_000_000.00m;
            public const string PriceRequiredErrorMessage = "Price is required!";

            public const int AddressMinLength = 10;
            public const int AddressMaxLength = 200;
            public const string AddressRequiredErrorMessage = "Address is required!";

            public const int CityMinLength = 3;
            public const int CityMaxLength = 100;
            public const string CityRequiredErrorMessage = "City is required!";

            public const int AreaMinValue = 1;
            public const int AreaMaxValue = 1_000_000;
            public const string AreaRequiredErrorMessage = "Area is required!";
        }
        public static class CategoryConstants
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 80;
        }

        public static class LocationConstants
        {
            public const int CityNameMinLength = 2;
            public const int CityNameMaxLength = 163;
        }

    }
}
