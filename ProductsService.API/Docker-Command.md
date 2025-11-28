

---

# 📦 **Products Microservice – Docker Setup & Database Initialization**

This guide explains how to run the **Products Microservice** and the **MySQL database** using Docker. It includes:

* MySQL container setup
* Database initialization using `init.sql`
* Building and running the microservice
* Example SQL schema & sample data

---

## 🚀 **1. Start MySQL Database Container**

This command runs a MySQL container with:

* Root password
* Custom hostname
* A named Docker container
* A shared Docker network
* Automatic database/table creation via `init.sql`

```sh
docker run -it ^
  -e MYSQL_ROOT_PASSWORD=admin ^
  -p 3307:3306 ^
  --hostname=mysql-host-productsmicroservice ^
  --name mysql-products ^
  --network productsmicroservice-network ^
  -v C:\mysql-init\init.sql:/docker-entrypoint-initdb.d/init.sql ^
  mysql:latest
```

### 🔍 What this does:

* Exposes MySQL on port **3307**
* Initializes the database using your SQL script
* Makes the DB accessible to the microservice via Docker network

---

## 🛠️ **2. Build & Push Products Microservice Image**

Run the following commands to build, tag, and push your image to Docker Hub:

```sh
docker build -t products-microservice:1.0 -f .\ProductsService.API\Dockerfile .
docker tag products-microservice:1.0 900325302/ecommerce-products-microservice:v1.0
docker push 900325302/ecommerce-products-microservice:v1.0
```

---

## ▶️ **3. Run the Products Microservice**

Use the image you pushed and run the microservice:

```sh
docker run -p 8080:8080 ^
  --network=productsmicroservice-network ^
  -e MYSQL_HOST=mysql-host-productsmicroservice ^
  -e MYSQL_PASSWORD=admin ^
  900325302/ecommerce-products-microservice:v1.0
```

### 🧠 Microservice Environment Variables:

| Variable         | Value                           |
| ---------------- | ------------------------------- |
| `MYSQL_HOST`     | mysql-host-productsmicroservice |
| `MYSQL_PASSWORD` | admin                           |

---

## 📂 **4. Database Initialization Script Mapping**

Your MySQL init script is mapped like this:

```
-v C:\mysql-init\init.sql:/docker-entrypoint-initdb.d/init.sql
```

> Anything inside `/docker-entrypoint-initdb.d/` runs **only on first container startup**.

---

## 🗄️ **5. Example init.sql (Database + Table + Sample Data)**

```sql
-- Create the database
CREATE DATABASE IF NOT EXISTS ecommerceproductsdatabase;
USE ecommerceproductsdatabase;

-- Create the products table
CREATE TABLE IF NOT EXISTS Products (
  ProductID char(36) NOT NULL,
  ProductName varchar(50) NOT NULL,
  Category varchar(50) DEFAULT NULL,
  UnitPrice decimal(10,2) DEFAULT NULL,
  QuantityInStock int DEFAULT NULL,
  PRIMARY KEY (ProductID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Insert 12 sample rows
INSERT INTO Products (ProductID, ProductName, Category, UnitPrice, QuantityInStock) VALUES
  ('1a9df78b-3f46-4c3d-9f2a-1b9f69292a77', 'Apple iPhone 15 Pro Max', 'Electronics', 1299.99, 50),
  ('2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17', 'Samsung Foldable Smart Phone 2', 'Electronics', 1499.99, 100),
  ('3f3e8b3a-4a50-4cd0-8d8e-1e178ae2cfc1', 'Ergonomic Office Chair', 'Furniture', 249.99, 25),
  ('4c9b6f71-6c5d-485f-8db2-58011a236b63', 'Coffee Table with Storage', 'Furniture', 179.99, 30),
  ('5d7e36bf-65c3-4a71-bf97-740d561d8b65', 'Samsung QLED 75 inch', 'Electronics', 1999.99, 20),
  ('6a14f510-72c1-42c8-9a5a-8ef8f3f45a0d', 'Running Shoes', 'Furniture', 49.99, 75),
  ('7b39ef14-932b-4c84-9187-55b748d2b28f', 'Anti-Theft Laptop Backpack', 'Accessories', 59.99, 60),
  ('8c5f6e73-68fc-49d9-99b4-aecc3706a4f4', 'LG OLED 65 inch', 'Electronics', 1499.99, 15),
  ('9e7e7085-6f4e-4921-8f15-c59f084080f9', 'Modern Dining Table', 'Furniture', 699.99, 10),
  ('10d7b110-ecdb-4921-85a4-58a5d1b32bf4', 'PlayStation 5', 'Electronics', 499.99, 40),
  ('11f2e86a-9d5d-42f9-b3c2-3e4d652e3df8', 'Executive Office Desk', 'Furniture', 299.99, 18),
  ('12b369b7-9101-41b1-a653-6c6c9a4fe1e4', 'Breville Smart Blender', 'HomeAppliances', 129.99, 50);
```

---

## 🧹 **6. Resetting the Database (if init.sql doesn't rerun)**

MySQL only executes init scripts **when the container is created for the first time**.

To rerun init.sql:

```sh
docker rm -f mysql-products
```

Then run the MySQL container again.

```



