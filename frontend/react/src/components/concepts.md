CONCEPTS EXPLAINED:

useEffect() VS effect()

```javascript

    // code runs on every render
    useEffect(() => {
        console.log('Trigger 1');
    });

    constructor() {  
        afterEveryRender(() => {
            console.log('Trigger 1'); // angular isolates state changes from DOM painting
        })
    }



    // code runs only on the first render
    useEffect(() => {
        console.log('Trigger 2');
    }, []);

    constructor() {
        effect(() => {
            console.log('Trigger 2');
        })
    }



    // runs on first render and any time a dependency changes 
    useEffect(() => {
        
    }, [prop, state])

    constructor() {
        effect(() => {
            const currentProp = this.myPropInput();
            const currentState = this.myLocalState();

            console.log(`Trigger 3: Values shifted to: ${currentProp}, ${currentState}`);
        })
    }
```

### 💡 The Effect Dependency & Trigger Matrix

| Desired Trigger | React Syntax | Angular Syntax |
| :--- | :--- | :--- |
| **Once on Mount** | `useEffect(fn, [])` | Place `effect(fn)` in constructor with **no signals read**. |
| **On Value Change** | `useEffect(fn, [value])` | Place `effect(fn)` in constructor and **execute `value()` inside it**. |
| **On Any Change** | `useEffect(fn)` | *Not applicable.* Angular only re-runs if a **tracked dependency signal explicitly changes**. |
