/*
Public client. Should only allow oidc auth and the group should be provided by 
the users's group. Needs access to both scopes
*/

resource "keycloak_openid_client" "public_client" {
  realm_id  = keycloak_realm.realm.id
  client_id = "public-client"

  name        = "public client"
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
  realm_id = keycloak_realm.realm.id

  client_id = keycloak_openid_client.public_client.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}

resource "keycloak_openid_group_membership_protocol_mapper" "public_group_membership_mapper" {
  realm_id  = keycloak_realm.realm.id
  client_id = keycloak_openid_client.public_client.id
  full_path=false
  name      = "group-membership-mapper"

  claim_name = "groups"
}

resource "keycloak_openid_client_optional_scopes" "client_optional_scopes_public" {
  realm_id  = keycloak_realm.realm.id
  client_id = keycloak_openid_client.public_client.id

  optional_scopes = [
    keycloak_openid_client_scope.cert-write-scope.name,
    keycloak_openid_client_scope.cert-read-scope.name
  ]
}
