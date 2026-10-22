import * as React from 'react';
import type { CommentLog } from './ApiConsumption.js';
import { useState } from 'react';

export function GoodPractices () {

    const [comments, setComments] = useState<CommentLog[]>([]);

    const badSort = () => {
        // This does NOT create a copy. It creates a new pointer to the EXACT same memory block.
        const tempArray = comments;
        
        // .sort() modifies the array right inside that memory address! (Mutation)
        tempArray.sort((a, b) => b.id - a.id);

        // React checks: Does 'tempArray' pointer === 'comments' pointer? 
        // Yes! The memory address never changed. React assumes nothing changed and SKIPS the re-render.
        // the data changed in heap memory but the browser screen wont update
        setComments(tempArray);
    }

    const goodSort = () => {
        // spread operator unpacks the records into brand new memory address on the heap
        const arrayCopy = [...comments];
        // Sorting the copy is perfectly safe; the original state remains untouched.
        arrayCopy.sort((a, b) => b.id - a.id);

        // React checks: Is this a new memory address pointer? 
        // Yes! React triggers a clean UI re-render instantly.
        setComments(arrayCopy);
    }

    return (
        <div>
        
        </div>
    );
}
