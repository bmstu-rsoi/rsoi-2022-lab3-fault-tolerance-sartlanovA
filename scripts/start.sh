mkdir rabbitmq
cd rabbitmq

mkdir log
chmod -R 777 ./log/

mkdir data
chmod -R 777 ./data/

docker-compose up -d