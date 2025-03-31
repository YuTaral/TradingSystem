CREATE TABLE portfolios (
    Id BIGSERIAL PRIMARY KEY,
    UserId BIGINT NOT NULL CHECK (UserId >= 1),
    Ticker VARCHAR(20) NOT NULL CHECK (LENGTH(Ticker) >= 1),
    Quantity INT NOT NULL CHECK (Quantity >= 0)
);