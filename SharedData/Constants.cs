namespace SharedData
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

        public enum RPCEventTypes
        {
            UPDATE_PORTFOLIO
        }

        public const string ORDER_CONTROLLER_ROUTE = "order";
        public const string PORTFOLIO_CONTROLLER_ROUTE = "portfolio";

        public const string POST_ORDER_ROUTE = "/add/{userId}";
        public const string GET_PORTFOLIO = "/get/{userId}";

        public const string KAFKA_BOOSTRAP_SERVER = "kafka:9092";
        public const string KAFKA_STOCK_PRICE_UPDATE_TOPIC = "stock-prices-update";
        public const string KAFKA_STOCK_PRICE_UPDATE_KEY = "stocks-dictionary";

        public const string RABBITMQ_HOST_NAME = "rabbitmq";
        public const int RABBITMQ_PORT_NUMBER = 5672;

        public const string RPC_QUEUE = "rpc_queue";
        public const string RPC_HOST = "localhost";

        public const string WRONG_URI = "The specified URI does not exist";
        public const string UNEXPECTED_ERROR_OCCURRED = "Unexpected error occurred while processing your request. Please try again";

        public const string STOCK_NOT_FOUND = "Stock not found";
        public const string ADD_ORDER_INVALID_DATA = "Add order failed - invalid data provided";
        public const string ADD_ORDER_FAILED_TO_GET_PRICE= "Add order failed - unable to get stock price";
        public const string USER_PORTFOLIO_NOT_FOUND = "User portfolio not found";
        public const string PORTFOLIO_STOCK_NOT_FOUND = "User does not own the stock";
        public const string PORTFOLIO_INVALID_QUANTITY = "Invalid stock quantity - sell quantity must be less or equal to the owned quantity";

        public const string USER_ID_ERROR = "User id must be greater than 0";
        public const string STOCK_TICKER_IS_MANDATORY_ERROR = "Stock\'s ticker is mandatory";
        public const string STOCK_TICKER_MAX_LEN_ERROR = "Stock\'s ticker is maximum length is 20 symbols";

        public const string ORDER_QUANTITY_RANGE_ERROR = "Order\'s quantity must be greater than 0";
        public const string ORDER_STOCK_PRICE_RANGE_ERROR = "Stock price cannot be negative";
        public const string ORDER_SIDE_ERROR = "Order side must be BUY or SELL";

        public const string PORTFOLIO_STOCK_QUANTITY_RANGE_ERROR = "Stock\'s quantity must not be negative";
    }
}
