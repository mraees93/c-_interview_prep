import { userData } from "./data.ts"

//Get a list of all users who are active and have a "vip" role.

// Define the nested order structure
// interface Order {
//   orderId: string;
//   total: number;
//   items: number;
//   status: 'delivered' | 'processing' | 'cancelled';
// }

// // Define the main user structure
// interface User {
//   id: number;
//   name: string;
//   email: string;
//   role: 'customer' | 'vip';
//   active: boolean;
//   loyaltyPoints: number;
//   orders: Order[];
// }

// const users: User[] = userData.filter(user => user.role === "vip" && user.active);
// console.log(users);

// extract all the email addresses.

const emailAddresses: string[] = userData.map(user => user.email);

console.log(emailAddresses);

