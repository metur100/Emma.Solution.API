# emma-api

Table Users {
  Id integer [primary key]
  Name varchar
  ProfilePicture varchar [note: 'Can be NULL']
  AddressId integer [ref: > Addresses.Id]
  ContactId integer [ref: > Contacts.Id]
  Ratings float [note: 'Can be NULL']
}

Table Addresses {
  Id integer [primary key]
  StreetAndNumber varchar
  PostalCode varchar
  City varchar
}

Table Contacts {
  Id integer [primary key]
  Email varchar
  PhoneNumber varchar
}

Table Vehicles {
  Id integer [primary key]
  UserId integer [ref: > Users.Id]
  Brand varchar
  PlateNumber varchar
  Description text [note: 'Can be NULL']
  BodyType varchar
  PayloadCapacity varchar
  CargoSpaceVolume varchar
  Length varchar
  Weight varchar
}

Table Freights {
  Id integer [primary key]
  UserId integer [ref: > Users.Id]
  LoadingAddressId varchar
  UnloadingAddressId varchar 
  TransportPrice varchar
  Status varchar
  Distance varchar
  GoodsDescription text
  GoodsType varchar
  GoodsWeight varchar
  GoodsLength varchar
  GoodsPrice varchar
  AvailabilityDate date
}


Table Offers {
  Id integer [primary key]
  VehicleId integer [ref: > Vehicles.Id]
  FreightId integer [ref: > Freights.Id]
  Status varchar
  NewTransportPrice varchar
  Comment varchar
}
