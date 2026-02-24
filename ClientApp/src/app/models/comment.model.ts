export interface Comment {
  id: number;
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  createdAt: string;
  imagePath?: string;
  textFilePath?: string;
  parentId?: number;
  replies: Comment[];
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}