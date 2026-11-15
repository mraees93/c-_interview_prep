import { userData } from "./data.ts"

/* SORT compare function to remember

const arr = [4, 5, 3, 1, 2, 6]

const sorted = [...arr].sort((a, b) => b - a);

function compareFn(a, b) {
  if(a is less than b by some ordering criterion) {

    return -1; // negative value indicates that 'a' should come before 'b'

  }
  else if (a is greater than b by the ordering criterion) {

    return 1; // positive value indicates that 'b' should come before 'a'
  }
  // a must be equal to b
  return 0;

• Negative result (-) → Keep the order. (No change needed, a stays on the left).
• Positive result (+) → Swap the order. (b moves to the left).
}
*/

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

//console.log(userData.every(user => user.email.startsWith("t")));


/*
Generate a list of profiles for active users only. Each profile must include the user's name 
and a calculated metric called averageOrderValue (total value of all their orders divided by 
the number of orders).
• Edge Case: If an active user has no orders, their averageOrderValue should be 0.
• Expected Output: An array/list of new objects containing only name and averageOrderValue.
*/

const getActiveUsers = () => {

  const activeUsers = userData.filter(user => user["active"]);

  const results = activeUsers.map((user) => {
    const userOrders = user["orders"];
    const totalOrderValue = userOrders.reduce((prev, curr) => prev + curr["total"], 0);

    if(userOrders.length > 0) {
      const averageOrderValue = totalOrderValue / userOrders.length;
      return { name: user.name,  "averageOrderValue": averageOrderValue };

    } else {

      return { name: user.name,  "averageOrderValue": 0 }
    }
  })

  return results;
}
console.log(getActiveUsers());
