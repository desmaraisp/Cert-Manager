/*
Public client. Should only allow oidc auth and the org should be provided by 
the users's role. Needs access to admin role
*/

resource "keycloak_openid_client" "public_client" {
  realm_id  = keycloak_realm.realm.id
  client_id = "public-client"

  name    = "public client"
  enabled = true

  web_origins           = ["*"]
  access_token_lifespan = 3600

  access_type = "PUBLIC"
  valid_redirect_uris = [
    "http://localhost:3000/oidc-callback"
  ]

  standard_flow_enabled = true
}

resource "keycloak_openid_audience_protocol_mapper" "audience_mapper" {
  realm_id = keycloak_realm.realm.id

  client_id = keycloak_openid_client.public_client.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}

resource "keycloak_role" "foo_role" {
  realm_id    = keycloak_realm.realm.id
  client_id   = keycloak_openid_client.public_client.id
  name        = "foo.Admin"
  description = "foo org admin role"
}
resource "keycloak_role" "bar_role" {
  realm_id    = keycloak_realm.realm.id
  client_id   = keycloak_openid_client.public_client.id
  name        = "bar.Admin"
  description = "bar org admin role"
}

resource "keycloak_openid_user_client_role_protocol_mapper" "user_client_role_mapper" {
  realm_id         = keycloak_realm.realm.id
  client_id        = keycloak_openid_client.public_client.id
  name             = "user-client-role-mapper"
  claim_name       = "roles"
  claim_value_type = "String"
  multivalued      = true
}
