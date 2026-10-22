BASICS:

Whats JSX and TSX?

It allows devs to write HTML-like syntax in JS or TS files.

---

Why use a functional component? And what's a hook?

Simplicity. Readability. Modern code practices especially with the use of hooks.

Hooks allow devs to hook into state and lifecycle methods.

---

Whats the 3 rules of hooks?

1. Hooks can only be called inside functional components.

2. Hooks can only be called at the top level of components.

3. Hooks cannot be conditional.

---

What's the lifecycle of react components?

Each component has a lifecycle consisting of 3 main stages:

1. Mounting - means adding elements in the DOM.

2. Updating - whenever there's a change in the components state or props.

3. Unmounting - when a component is removed from the DOM.

---

CONCEPTS EXPLAINED:

props vs state

Props (or inputs) are external and immutable, they are any data passed down from parent component. The child component is forbidden from changing it directly
State is internal and mutable, it's data owned and changed by the component itself (e.g searchTerm)

```javascript

```

```javascript

```

### 📊 Local State vs. Props Architecture Matrix

| Capability | React Paradigm | Modern Angular Paradigm (22+) |
| :--- | :--- | :--- |
| **Local State** | `useState('initial')` | `signal('initial')` |
| **Updating State** | `setState('new')` | `mySignal.set('new')` or `.update()` |
| **Receiving Props** | Functional Arguments: `function Child({ item })` | The **`input()`** signal primitive |
| **Reading Props** | Read directly as raw value: `{item.name}` | Read reactive value as getter: `{{ item().name }}` |
| **Required Props** | Enforced manually via TypeScript interface contracts | Enforced natively by compiler rules: `input.required()` |

---

useEffect() VS effect()

The useEffect Hook allows you to perform side effects in your components.

Some examples of side effects are: fetching data, directly updating the DOM, and timers.

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

---

### UseMemo:

To improve performance it only runs when one of its dependencies update and it can be used to keep resource intensive functions from unecessary running.

useMemo returns a memoized value.

useCallback returns a memoized function.