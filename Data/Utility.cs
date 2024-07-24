using OtlobLap.Models;
using OtlobLap.Resources;

namespace OtlobLap.Data
{
    public static class Utility
    {
        public static  LocalizationService localizationService;
        public enum Roles
        {
            Admin,
            Seller,
            Customer,
            Blocked
        }
        
        public enum Modules
        {
          Home , 
          Order , 
          Account , 
          Product , 
          Category,
          Discount ,
          Review , 
          WishList,
          Cart , 
          User , 

        }
        public static class Permissions
        {
            public static class User
            {
                public const string Add = "Permissions.User.Add";
                public const string Edit = "Permissions.User.Edit";
                public const string Remove = "Permissions.User.Remove";
                public const string View = "Permissions.User.View";
            }
            public static class Product
            {
                public const string Add = "Permissions.Product.Add";
                public const string Edit = "Permissions.Product.Edit";
                public const string Remove = "Permissions.Product.Remove";
                public const string View = "Permissions.Product.View";
            }

            public static class Order
            {
                public const string Add = "Permissions.Order.Add";
                public const string Edit = "Permissions.Order.Edit";
                public const string Remove = "Permissions.Order.Remove";
                public const string View = "Permissions.Order.View";
            }

            public static class Account
            {
                public const string Add = "Permissions.Account.Add";
                public const string Edit = "Permissions.Account.Edit";
                public const string Remove = "Permissions.Account.Remove";
                public const string View = "Permissions.Account.View";
            }

            public static class Category
            {
                public const string Add = "Permissions.Category.Add";
                public const string Edit = "Permissions.Category.Edit";
                public const string Remove = "Permissions.Category.Remove";
                public const string View = "Permissions.Category.View";
            }

            public static class Discount
            {
                public const string Add = "Permissions.Discount.Add";
                public const string Edit = "Permissions.Discount.Edit";
                public const string Remove = "Permissions.Discount.Remove";
                public const string View = "Permissions.Discount.View";
            }

            public static class Review
            {
                public const string Add = "Permissions.Review.Add";
                public const string Edit = "Permissions.Review.Edit";
                public const string Remove = "Permissions.Review.Remove";
                public const string View = "Permissions.Review.View";
            }

            public static class WishList
            {
                public const string Add = "Permissions.WishList.Add";
                public const string Edit = "Permissions.WishList.Edit";
                public const string Remove = "Permissions.WishList.Remove";
                public const string View = "Permissions.WishList.View";
            }

            public static class Cart
            {
                public const string Add = "Permissions.Cart.Add";
                public const string Edit = "Permissions.Cart.Edit";
                public const string Remove = "Permissions.Cart.Remove";
                public const string View = "Permissions.Cart.View";
            }

            public static class Home
            {
                public const string View = "Permissions.Home.View";
            }
        }

        public const string NoImageUrl = "no-image.jpg";
        public static decimal CalculateDiscountedPrice(decimal originalPrice, int discountPercentage)
			{
				decimal discountMultiplier = 1 - (discountPercentage / 100m); 
				decimal discountedPrice = originalPrice * discountMultiplier;
				return Math.Round(discountedPrice, 2, MidpointRounding.AwayFromZero);
			}
  
        public static string ConvertPriceToShort(decimal price)
        {
            string[] suffixes = { "", "ألف", "مليون", "مليار", "تريليون" }; // Define the suffixes for thousand, million, billion, trillion, etc.

            int suffixIndex = 0;
            decimal adjustedPrice = price;

            // Loop until the price is smaller than 1000 or there are no more suffixes
            while (adjustedPrice >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                adjustedPrice /= 1000;
                suffixIndex++;
            }

            // Format the price with currency formatting and the appropriate suffix
            string formattedPrice = adjustedPrice.ToString("C"); 
            return $"{formattedPrice} {suffixes[suffixIndex]}";
        }

        public static string CalculateRemainingTime(DateTime endDate)
        {
            TimeSpan remainingTime = endDate - DateTime.Now;

            if (remainingTime.TotalMilliseconds <= 0)
            {
                return "منتهي";
            }

            int days = (int)remainingTime.TotalDays;
            int hours = remainingTime.Hours;
            int minutes = remainingTime.Minutes;

            return $"{days} يوم و {hours} ساعة و {minutes} دقيقة";
        }

        public static string OrderStatusSpan(OrderStatus? orderStatus)
        {
            string htmlSpan;

                string word = GetTranslation(orderStatus.ToString());
            switch (orderStatus)
            {
                case OrderStatus.New:

                    htmlSpan = $"<span class=\" badge btn btn-info\">{word}</span>";
                    break;

                case OrderStatus.InProgress:
                    htmlSpan = $"<span class=\" badge btn btn-warning\">{word}</span>";
                    break;

                case OrderStatus.Arrived:
                    htmlSpan = $"<span class=\" badge btn btn-success\">{word}</span>";
                    break;

                case OrderStatus.Rejected:
                    htmlSpan = $"<span class=\" badge btn btn-danger\">{word}</span>";
                    break;

                default:
                    htmlSpan = $"<span class=\" badge btn btn-secondary\">Unknown</span>";
                    break;
            }

            return htmlSpan;
        }
        public static  string GetDirection(string culture)
        {
            if (culture.StartsWith("ar"))
            {
                return "rtl";
            }
            else
            {
                return "ltr";
            }
        }

        // Indexer to get the translation using the localization service
        public static string GetTranslation(string key)
        {
            if (localizationService == null)
            {
                throw new InvalidOperationException("Localization service has not been set.");
            }

            return localizationService.GetLocalizedHtmlString(key);
        }
    }

}