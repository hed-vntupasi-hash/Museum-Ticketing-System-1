CREATE DATABASE museum_ticketing_system;
USE museum_ticketing_system;

CREATE TABLE events (
  event_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  event_name VARCHAR(100) DEFAULT NULL,
  description text,
  location VARCHAR(100) DEFAULT NULL,
  start_date DATE DEFAULT NULL,
  end_date DATE DEFAULT NULL
);

CREATE TABLE ticket_types (
  ticket_type_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  type_name VARCHAR(50) DEFAULT NULL,
  price DECIMAL(10,2) DEFAULT NULL,
  description VARCHAR(255) DEFAULT NULL
);

CREATE TABLE tickets (
  ticket_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  ticket_type_id INT DEFAULT NULL,
  event_id INT DEFAULT NULL,
  visitor_id INT DEFAULT NULL,
  status VARCHAR(20) DEFAULT 'active',
  created_at TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (ticket_type_id) REFERENCES ticket_types(ticket_type_id),
  FOREIGN KEY (event_id) REFERENCES events(event_id)
);

CREATE TABLE staff (
  staff_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  username VARCHAR(45) DEFAULT NULL,

  first_name VARCHAR(50) DEFAULT NULL,
  last_name VARCHAR(50) DEFAULT NULL,
  
  role ENUM('Staff','Admin') DEFAULT 'Staff',
  email VARCHAR(100) DEFAULT NULL,
  phone VARCHAR(20) DEFAULT NULL,
  password VARCHAR(100) DEFAULT NULL,
  
  UNIQUE KEY username_UNIQUE (username)
);

CREATE TABLE ticket_sales (
  sale_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  ticket_id INT DEFAULT NULL,
  staff_id INT DEFAULT NULL,
  sale_date TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  quantity INT DEFAULT NULL,
  total_price DECIMAL(10,2) DEFAULT NULL,
  FOREIGN KEY (ticket_id) REFERENCES tickets(ticket_id),
  FOREIGN KEY (staff_id) REFERENCES staff(staff_id)
);

CREATE TABLE payments (
  payment_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  sale_id INT DEFAULT NULL,
  payment_method VARCHAR(50) DEFAULT NULL,
  payment_status VARCHAR(20) DEFAULT NULL,
  payment_date TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (sale_id) REFERENCES ticket_sales (sale_id)
);

CREATE TABLE visitors (
  visitor_id INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  first_name VARCHAR(50) DEFAULT NULL,
  last_name VARCHAR(50) DEFAULT NULL,
  email VARCHAR(100) DEFAULT NULL,
  phone VARCHAR(20) DEFAULT NULL,
  created_at TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP
);