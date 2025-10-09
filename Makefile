.PHONY: up start stop down clean-volume

up:
	docker compose -f compose.yaml up -d

start:
	docker compose -f compose.yaml start

stop:
	docker compose -f compose.yaml stop

down:
	docker compose -f compose.yaml down && docker volume prune -f && docker volume rm markitdoneapi_pgdata

rmv-images:
	docker system prune -a --volumes -f