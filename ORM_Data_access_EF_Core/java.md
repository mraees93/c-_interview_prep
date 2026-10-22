```java

    List<Integer> points = new ArrayList<>();
    points.add(10);
    points.add(20);
    points.add(30);

    Stream<Integer> query = points.stream().filter(p -> p > 15);

    List<Integer> materialize = query.toList();

    points.add(40);

    System.out.println(materialize.size());
    System.out.println(points.size());

```

```java

    List<Integer> transactions = new ArrayList<>();

    int result = transactions.stream()
                             .filter(t -> t > 100)
                             .findFirst() // returns an optional Integer
                             .orElse(0); // if empty, returns default zero

    System.out.println(result);


    /* Performance Note: Primitive Specialisation

    If you are processing highly intensive or massive mathematical arrays, you can use an IntStream in Java. This avoids the object overhead of Integer wrapping on the heap and works directly with primitive memory layouts: */

    int result = transactions.stream()
                             .mapToInt(Integer::intValue) // converts to primitive IntStream
                             .filter(t -> t > 100)
                             .findFirst() // Returns an optional Integer
                             .orElse(0);

```

```java

    record User(int id, String role) {} // lightweight immutable record

    User[] users = {
        new User(1, "Admin"),
        new User(2, "User"),
        new User(3, "Guest")
    }

    List<User> target = List.of(users).stream()
        .filter(u -> u.id() == 2 || "Admin".equals(u.Role()))
        .toList();

    target.forEach(System.out::println);
```