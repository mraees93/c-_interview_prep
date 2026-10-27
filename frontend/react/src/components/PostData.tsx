import * as React from 'react';
import { useState, type ChangeEvent, type SubmitEvent } from 'react';

export default function PostData () {
    const [name, setName] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [number, setNumber] = useState<number>();

    const handleNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setName(e.target.value);
    }

    const handleEmailChange = (e: ChangeEvent<HTMLInputElement>) => {
        setEmail(e.target.value);
    }

    const handleNumberChange = (e: ChangeEvent<HTMLInputElement>) => {
        setNumber(Number(e.target.value));
    }

    const handleSubmitForm = (e: SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        alert(`${name}, ${email}, ${number}`);
    }

  return (
      <form onSubmit = {handleSubmitForm}>
      <label>Enter your name:
        <input
          type="text" 
          value={name}
          onChange={handleNameChange}
        />
      </label>
      <p>Current value: {name}</p>

      <label>Enter your email:
        <input
          type="email" 
          value={email}
          onChange={handleEmailChange}
        />
      </label>
      <p>Current value: {email}</p>

      <label>Enter your number:
        <input
          type="number" 
          value={number}
          onChange={handleNumberChange}
        />
      </label>
      <p>Current value: {number}</p>
      <input type="submit" />
    </form>
  );
}
