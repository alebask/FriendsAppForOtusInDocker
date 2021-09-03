FROM mysql:latest

#create database on first start if container
COPY friendsdb/setup/GenerateSchema.sql docker-entrypoint-initdb.d/GenerateSchema.sql
