CREATE TABLE orders (
    Id BIGSERIAL PRIMARY KEY,
    UserId BIGINT NOT NULL CHECK (UserId >= 1),
    Ticker VARCHAR(20) NOT NULL CHECK (LENGTH(Ticker) >= 1),
    Quantity INT NOT NULL CHECK (Quantity >= 1),
    Side VARCHAR(10) NOT NULL CHECK (LENGTH(Side) >= 3), 
    Price DECIMAL NOT NULL CHECK (Price >= 0.1)
);