resource "keycloak_openid_client" "client_credentials_clients" {
  for_each  = keycloak_realm.realms
  realm_id  = each.value.id
  client_id = "${each.value.id}-client"

  name        = "${each.value.id} client"
  enabled     = true

  access_type = "CONFIDENTIAL"

  client_secret                = "BAD_CLIENT_SECRET"
  service_accounts_enabled     = true
}
resource "keycloak_openid_audience_protocol_mapper" "client_credentials_audience_mapper" {
  for_each = keycloak_openid_client.client_credentials_clients
  realm_id = each.value.realm_id

  client_id = each.value.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}
resource "keycloak_openid_client_optional_scopes" "client_optional_scopes" {
  for_each  = var.KEYLCOAK_REALMS
  realm_id  = keycloak_realm.realms[each.value].id
  client_id = keycloak_openid_client.client_credentials_clients[each.value].id

  optional_scopes = [
    keycloak_openid_client_scope.cert-write-scope[each.value].name,
    keycloak_openid_client_scope.cert-read-scope[each.value].name
  ]
}
