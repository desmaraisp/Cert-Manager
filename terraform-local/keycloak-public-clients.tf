resource "keycloak_openid_client" "public_clients" {
  for_each  = keycloak_realm.realms
  realm_id  = each.value.id
  client_id = "${each.value.id}-public-client"

  name        = "${each.value.id} public client"
  enabled     = true

  web_origins = ["*"]
  access_token_lifespan = 3600

  access_type = "PUBLIC"
  valid_redirect_uris = [
    "http://localhost:3000/oidc-callback"
  ]

  standard_flow_enabled        = true
}

resource "keycloak_openid_audience_protocol_mapper" "audience_mapper" {
  for_each = keycloak_openid_client.public_clients
  realm_id = each.value.realm_id

  client_id = each.value.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}

resource "keycloak_openid_client_optional_scopes" "client_optional_scopes_public" {
  for_each  = var.KEYLCOAK_REALMS
  realm_id  = keycloak_realm.realms[each.value].id
  client_id = keycloak_openid_client.public_clients[each.value].id

  optional_scopes = [
    keycloak_openid_client_scope.cert-write-scope[each.value].name,
    keycloak_openid_client_scope.cert-read-scope[each.value].name
  ]
}
