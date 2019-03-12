$ docker service create \
     --name dns-updater \
     --replicas 1 \
     --secret source=dns_updater_key,target=dns_updater_key \
     --secret source=dns_updater_zone,target=dns_updater_zone \
     -e DNS_UPDATER_EMAIL="mycloudflareaccount@me.com" \
     -e DNS_UPDATER_KEY_FILE="/run/secrets/dns_updater_key" \
     -e DNS_UPDATER_ZONE_FILE="/run/secrets/dns_updater_zone" \
     dungtri/docker-cloudflare-dns-updater:latest