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