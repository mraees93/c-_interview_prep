export const userData = [
  {
    "id": 1,
    "name": "Alex Morgan",
    "email": "alex@example.com",
    "role": "customer",
    "active": true,
    "loyaltyPoints": 150,
    "orders": [
      { "orderId": "A101", "total": 45.99, "items": 3, "status": "delivered" },
      { "orderId": "A102", "total": 12.50, "items": 1, "status": "delivered" }
    ]
  },
  {
    "id": 2,
    "name": "Taylor Swift",
    "email": "taylor@example.com",
    "role": "vip",
    "active": true,
    "loyaltyPoints": 850,
    "orders": [
      { "orderId": "T201", "total": 120.00, "items": 5, "status": "delivered" },
      { "orderId": "T202", "total": 340.50, "items": 12, "status": "processing" }
    ]
  },
  {
    "id": 3,
    "name": "Jordan Blake",
    "email": "jordan@example.com",
    "role": "customer",
    "active": false,
    "loyaltyPoints": 40,
    "orders": [
      { "orderId": "J301", "total": 89.99, "items": 2, "status": "cancelled" }
    ]
  },
  {
    "id": 4,
    "name": "Sam Smith",
    "email": "sam@example.com",
    "role": "vip",
    "active": true,
    "loyaltyPoints": 500,
    "orders": []
  },
  {
    "id": 5,
    "name": "Casey Jones",
    "email": "casey@example.com",
    "role": "customer",
    "active": true,
    "loyaltyPoints": 210,
    "orders": [
      { "orderId": "C401", "total": 15.75, "items": 1, "status": "delivered" },
      { "orderId": "C402", "total": 65.00, "items": 4, "status": "delivered" },
      { "orderId": "C403", "total": 110.00, "items": 2, "status": "delivered" }
    ]
  }
] 
//as unknown as User[];
