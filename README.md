# Hexagonal_API-Ebay
 A practice project using Hexagonal Architecture with SQLite and data from Kaggle!

About the Project

This API was developed as a practice project by Santiago Celestre, using Hexagonal Architecture and data extracted from Kaggle.
It serves as an experiment in designing and testing a structured API while working with SQLite as a lightweight embedded database.

 Key Features
 
✅ Hexagonal Architecture → Clean and scalable design for maintainable code.

✅ Stock Evolution Tracking → Detects major events in eBay’s stock history.

✅ Historical Event Analysis → Associates price changes with real-world events.

✅ JSON and Excel Reports → Supports structured data output for easy analysis.


Available Endpoints

GET	/api/ebaystock/mapa-evolutivo-json	Returns a JSON with eBay’s stock evolution, financial events, and expert opinions.

GET	/api/ebaystock/informe-volumen	Generates an Excel file with stock transaction volume distribution.

GET	/api/ebaystock/informe-tendencias	Generates an Excel report with stock trends and key insights.

Technologies Used

C# .NET 6+

SQLite (lightweight embedded database)

Entity Framework Core (ORM for database management)

EPPlus (Excel report generation)

Newtonsoft.Json (JSON processing)

Swagger (API documentation)



This API was built to experiment with Hexagonal Architecture while working with real-world financial data.

The dataset was extracted from Kaggle, and SQLite was chosen as the database for its simplicity and ease of integration.

The project serves as a practical exercise in building scalable and modular APIs using .NET 6+.


Developed by Santiago Celestre.


