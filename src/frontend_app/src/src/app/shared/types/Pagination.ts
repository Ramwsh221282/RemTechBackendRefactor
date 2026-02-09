import { HttpParams } from '@angular/common/http';

export type Pagination = {
  page: number;
  pageSize: number;
};

export const PaginationWithPage: (
  page: number,
  pagination: Pagination,
) => Pagination = (page: number, pagination: Pagination): Pagination => {
  return { ...pagination, page: page };
};

export const PaginationWithPageSize: (
  pageSize: number,
  pagination: Pagination,
) => Pagination = (pageSize: number, pagination: Pagination): Pagination => {
  return { ...pagination, pageSize: pageSize };
};

export const NewPagination: (page: number, size: number) => Pagination = (
  page: number,
  size: number,
): Pagination => {
  return { page: page, pageSize: size };
};

export class PaginationService {
  public static initialized(page: number, size: number): Pagination {
    return { page: page, pageSize: size };
  }

  public static updatePage(original: Pagination, page: number): Pagination {
    return { ...original, page: page };
  }
}

export const mapToHttpParameters = (pagination: Pagination): HttpParams => {
  return new HttpParams()
    .append('page', String(pagination.page))
    .append('pageSize', String(pagination.pageSize));
};
