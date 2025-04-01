# Trading System
1. Install Windows Subsystem for Linux - https://learn.microsoft.com/en-us/windows/wsl/install
2. Install Docker for Desktop - https://docs.docker.com/desktop/setup/install/windows-install/
3. Start Docker and go to Settings, General and verify "Use the WSL 2 based engine" is selected
4. Donwload the solution, open it in Visual Sutdio and build the solution
5. Open Docker's terminal and navigate to the solution's root folder
6. Run the command "docker-compose up" and wait until all services are started (5 - 10 mins)
7. Once all services are started, you can use a tool like Postman to send requests
8. To add order, send POST request to "http://localhost:5000/portfolio/get/{userId}" with content:
	{
		"Ticker": "TSLA",
		"Quantity": 1000,
		"Side": "BUY"
	}
9. To view user's portfolio value, send GET request to "http://localhost:5000/portfolio/get/{userId}"
11. To sell a steck specify "Side": "SELL" in the POST request
12. Available stocks - AAPL, TSLA, NVDA, MFST, AMZN
