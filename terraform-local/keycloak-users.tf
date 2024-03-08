resource "keycloak_user" "admin_user" {
  realm_id = keycloak_realm.realm.id
  username = "root"
  enabled  = true

  email = "root@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
}
resource "keycloak_user_groups" "admin_groups" {
  realm_id = keycloak_realm.realm.id
  user_id = keycloak_user.admin_user.id

  group_ids  = [
    keycloak_group.foo.id,
    keycloak_group.bar.id
  ]
}


resource "keycloak_user" "foo_user" {
  realm_id = keycloak_realm.realm.id
  username = "foo"
  enabled  = true

  email = "foo@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
}
resource "keycloak_user_groups" "foo_groups" {
  realm_id = keycloak_realm.realm.id
  user_id = keycloak_user.foo_user.id

  group_ids  = [
    keycloak_group.foo.id
  ]
}

resource "keycloak_user" "bar_user" {
  realm_id = keycloak_realm.realm.id
  username = "bar"
  enabled  = true

  email = "bar@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
}
resource "keycloak_user_groups" "bar_groups" {
  realm_id = keycloak_realm.realm.id
  user_id = keycloak_user.bar_user.id

  group_ids  = [
    keycloak_group.bar.id
  ]
}

