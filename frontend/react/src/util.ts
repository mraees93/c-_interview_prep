export enum TMDBGenre {
  Action = 28,
  Adventure = 12,
  Animation = 16,
  Comedy = 35,
  Crime = 80,
  Documentary = 99,
  Drama = 18,
  Family = 10751,
  Fantasy = 14,
  History = 36,
  Horror = 27,
  Music = 10402,
  Mystery = 9648,
  Romance = 10749,
  ScienceFiction = 878,
  TVMovie = 10770,
  Thriller = 53,
  War = 10752,
  Western = 37
}
/*
Reverse mapping. Accessing by number gives you the string key (TMDBGenre[37] → "Western")

Reverse mapping only works for numeric enums. If you ever change the 
values to strings, TMDBGenre["37"] will return undefined.
*/