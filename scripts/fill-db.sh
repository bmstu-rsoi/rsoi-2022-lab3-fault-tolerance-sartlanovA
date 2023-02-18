docker ps -a

docker exec -i postgres psql -U program cars < test/cars.dump.sql
docker exec -i postgres psql -U program payments < test/payment.dump.sql
docker exec -i postgres psql -U program rentals < test/rental.dump.sql