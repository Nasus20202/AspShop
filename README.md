# Online store - ASP.NET Core

AspShop is a simple online store web application. It's created with ASP.NET Core 5.0. The default database is mySQL, but it can be easily changed. 
I created this application as a educational project. It is mostly complete. 

You can browse products, which are divided into subcategories and categories. You can also sort and filter products. The simple account system is fully implemented and it's possible to create new account,
add some information and then change it. The app has also an admin panel, which can be used to easily change, add and remove products, or even update
shop structure. Minimum required role to access admin panel is "employee" (other: "editor", "manager", "admin"). To firstly access admin panel you have to 
update your role using SQL. Orders system is also fully implemented. However, there is no payment avaiable. 


## Configuration

.NET CLI
```
dotnet user-secrets init
dotnet user-secrets set "production_string" "Server = XXX; Database = XXX; Uid = XXX; Pwd = XXX;"
dotnet user-secrets set "development_string" "Server = XXX; Database = XXX; Uid = XXX; Pwd = XXX;"
```

### Database 
Filters can be added by changing the "Tags" value in admin panel subcategory settings. To create a new filter, just add "filterName:filterType" to "Tags".
Avaiable types are "string" and "int", string is a list of all the avaiable values, and int is a numercial value range. 
Then you have to add a filter value to the products, just add "filterName:value" to the "Tags" value of the product.
The tags shoud be separated with semicolon.

#### Example
Add "color:string;width:int" to subcategory, and "color:red;width:20" or "color:yellow;width:12" to products in this subcategory.
