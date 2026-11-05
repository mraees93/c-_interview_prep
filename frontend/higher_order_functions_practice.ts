import { userData } from "./data.ts"

//Get a list of all users who are active and have a "vip" role.

//Define the nested order structure
interface Order {
  orderId: string;
  total: number;
  items: number;
  status: 'delivered' | 'processing' | 'cancelled';
}

// Define the main user structure
interface User {
  id: number;
  name: string;
  email: string;
  role: 'customer' | 'vip';
  active: boolean;
  loyaltyPoints: number;
  orders: Order[];
}

// const users: User[] = userData.filter(user => user.role === "vip" && user.active);
// console.log(users);

// extract all the email addresses.

const emailAddresses: string[] = userData.map(user => user.email);

//console.log(emailAddresses);

//Find the total monetary value of all orders that are either "delivered" 
// or "processing", but only for users who have a "vip" role.

const vipTotalSpending = userData
                            .filter(user => user.role === "vip")
                            .flatMap(user => user.orders)
                            .filter(order => order.status === "delivered" || order.status === "processing")
                            .reduce((sum, order) => sum + order.total, 0);

console.log(vipTotalSpending);

const vipTotal = userData
  .filter(user => user.role === "vip")
  .reduce((acc, user) => {
    // Sum up the valid orders for the current user
    const userOrderSum = user.orders.reduce((sum, order) => {
      if (order.status === "delivered" || order.status === "processing") {
        return sum + order.total;
      }
      return sum; // Crucial: Always return the accumulator if condition fails!
    }, 0);
    
    return acc + userOrderSum;
  }, 0);

//console.log(vipTotal);

console.log(userData.every(user => user.email.startsWith("t")));
