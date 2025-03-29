namespace Shared
{
    /// <summary>
    ///     Constants class containing messages
    /// </summary>
    public static class Constants
    {
        public enum OrderSide
        {
            BUY,
            SELL
        }

        public const string ORDER_CONTROLLER_ROUTE = "/api/order";

        public const string POST_ORDER_ROUTE = ORDER_CONTROLLER_ROUTE + "/add/{userId}";
        public const string POST_ORDER_ROUTE_get = ORDER_CONTROLLER_ROUTE + "/get";

        public const string KAFKA_BOOSTRAP_SERVER = "localhost:9092";
        public const string KAFKA_STOCK_PRICE_UPDATE_TOPIC = "stock-prices-update";
        public const string KAFKA_STOCK_PRICE_UPDATE_KEY = "stocks-dictionary";

        public const string STOCK_NOT_FOUND = "Stock not found";
        public const string ADD_ORDER_INVALID_DATA = "Add order failed - invalid data provided";
        public const string ADD_ORDER_FAILED_TO_GET_PRICE= "Add order failed - unable to get stock price";

        public const string ORDER_TICKER_IS_MANDATORY_ERROR = "Order\'s ticker is mandatory";
        public const string ORDER_TICKER_MAX_LEN_ERROR = "Order\'s Ticker is maximum length is 20 symbols";
        public const string ORDER_QUANTITY_RANGE_ERROR = "Order\'s quantity must be greater than 0";
        public const string ORDER_STOCK_PRICE_RANGE_ERROR = "Stock price cannot be negative";
        public const string ORDER_USER_ID_ERROR = "User id must be greater than 0";
        public const string ORDER_SIDE_ERROR = "Order side must be BUY or SELL";
    }
}
