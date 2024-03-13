import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const CertificateModel = z
  .object({
    isCertificateAuthority: z.boolean(),
    certificateName: z.string().nullable(),
    tags: z.array(z.string()).nullable(),
    certificateDescription: z.string().nullable(),
    requirePrivateKey: z.boolean(),
  })
  .partial();
const CertificateModelWithId = z
  .object({
    isCertificateAuthority: z.boolean(),
    certificateName: z.string().nullable(),
    tags: z.array(z.string()).nullable(),
    certificateDescription: z.string().nullable(),
    requirePrivateKey: z.boolean(),
    certificateId: z.string().uuid(),
  })
  .partial();
const ProblemDetails = z
  .object({
    type: z.string().nullable(),
    title: z.string().nullable(),
    status: z.number().int().nullable(),
    detail: z.string().nullable(),
    instance: z.string().nullable(),
  })
  .partial()
  .passthrough();
const CertificateUpdateModel = z
  .object({
    newCertificateName: z.string().nullable(),
    newTags: z.array(z.string()).nullable(),
    newCertificateDescription: z.string().nullable(),
  })
  .partial();
const CertificateRenewalScheduleModel = z
  .object({
    subscriptionId: z.string().uuid(),
    certificateDuration: z.string(),
    certificateSubject: z.string().nullable(),
    scheduledRenewalTime: z.string().datetime({ offset: true }),
    destinationCertificateId: z.string().uuid(),
    parentCertificateId: z.string().uuid(),
  })
  .partial();
const CertificateRenewalSubscriptionModelWithId = z
  .object({
    certificateDuration: z.string(),
    certificateSubject: z.string().nullable(),
    renewXDaysBeforeExpiration: z.number().int().gte(1).lte(365),
    destinationCertificateId: z.string().uuid(),
    parentCertificateId: z.string().uuid(),
    subscriptionId: z.string().uuid(),
  })
  .partial();
const CertificateRenewalSubscriptionModel = z
  .object({
    certificateDuration: z.string(),
    certificateSubject: z.string().nullable(),
    renewXDaysBeforeExpiration: z.number().int().gte(1).lte(365),
    destinationCertificateId: z.string().uuid(),
    parentCertificateId: z.string().uuid(),
  })
  .partial();
const CertificateVersionModel = z
  .object({
    activationDate: z.string().datetime({ offset: true }),
    cn: z.string().nullable(),
    expiryDate: z.string().datetime({ offset: true }),
    issuerName: z.string().nullable(),
    thumbprint: z.string().nullable(),
    rawCertificate: z.string().nullable(),
    certificateId: z.string().uuid(),
    certificateVersionId: z.string().uuid(),
  })
  .partial();

export const schemas = {
  CertificateModel,
  CertificateModelWithId,
  ProblemDetails,
  CertificateUpdateModel,
  CertificateRenewalScheduleModel,
  CertificateRenewalSubscriptionModelWithId,
  CertificateRenewalSubscriptionModel,
  CertificateVersionModel,
};

const endpoints = makeApi([
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateRenewalSchedules",
    alias: "GetCertificateRenewalSchedules",
    requestFormat: "json",
    parameters: [
      {
        name: "MinimumUtcScheduledTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumUtcScheduledTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateRenewalScheduleModel),
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateRenewalSubscriptions",
    alias: "GetCertificateRenewalSubscriptions",
    requestFormat: "json",
    parameters: [
      {
        name: "CertificateIds",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateRenewalSubscriptionModelWithId),
  },
  {
    method: "post",
    path: "/:organizationId/api/v1/CertificateRenewalSubscriptions",
    alias: "CreateCertificateRenewalSubscriptions",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CertificateRenewalSubscriptionModel,
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateRenewalSubscriptionModelWithId,
  },
  {
    method: "delete",
    path: "/:organizationId/api/v1/CertificateRenewalSubscriptions/:id",
    alias: "DeleteCertificateRenewalSubscription",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "post",
    path: "/:organizationId/api/v1/Certificates",
    alias: "CreateCertificate",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CertificateModel,
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateModelWithId,
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/Certificates",
    alias: "GetAllCertificates",
    requestFormat: "json",
    parameters: [
      {
        name: "TagsToSearch",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "TagsSearchBehavior",
        type: "Query",
        schema: z.enum(["MatchAll", "MatchAny"]).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateModelWithId),
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/Certificates/:id",
    alias: "GetCertificateById",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateModelWithId,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "delete",
    path: "/:organizationId/api/v1/Certificates/:id",
    alias: "DeleteCertificateById",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "patch",
    path: "/:organizationId/api/v1/Certificates/:id",
    alias: "EditCertificateById",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CertificateUpdateModel,
      },
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateModelWithId,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "post",
    path: "/:organizationId/api/v1/CertificateVersion",
    alias: "CreateCertificateVersion",
    requestFormat: "form-data",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z
          .object({ Certificate: z.instanceof(File) })
          .partial()
          .passthrough(),
      },
      {
        name: "Password",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "CertificateId",
        type: "Query",
        schema: z.string().uuid().optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateVersionModel,
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateVersions",
    alias: "GetCertificateVersions",
    requestFormat: "json",
    parameters: [
      {
        name: "CertificateIds",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "MinimumUtcExpirationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumUtcExpirationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MinimumUtcActivationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumUtcActivationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateVersionModel),
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateVersions/:id",
    alias: "GetCertificateVersionById",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateVersionModel,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "delete",
    path: "/:organizationId/api/v1/CertificateVersions/:id",
    alias: "DeleteCertificateVersion",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
