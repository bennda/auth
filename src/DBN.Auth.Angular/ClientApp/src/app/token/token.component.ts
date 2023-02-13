import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-token',
  templateUrl: './token.component.html',
})
export class TokenComponent implements OnInit {
  status: string = '';
  statusText: string = '';
  formData: FormGroup | undefined;
  response: Token = {
    access_token: '',
    refresh_token: '',
    id_token: '',
    token_type: '',
    scope: '',
    expires_in: 0
  };
  request: Request = {
    grant_type: '',
    domain: '',
    client_id: '',
    client_secret: '',
    audience: '',
    username: '',
    password: '',
    scope: '',
    code: '',
    redirect_uri: '',
    refresh_token: ''
  };

  ngOnInit(): void {
    this.formData = this.builder.group({
      Fullname: new FormControl('', [Validators.required]),
      Email: new FormControl('', [Validators.required]),
      Comment: new FormControl('', [Validators.required])
    });
  }

  constructor(private builder: FormBuilder, private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  selectGrantTypeHandler(event: any) {
    this.status = '';
    this.request.grant_type = event.target.value;
  }

  public createToken() {
    this.status = '';
    this.statusText = '';
    this.http.post<Token>(this.baseUrl + 'api/auth', this.request).subscribe(result => {
      this.response = result;
      this.status = 'ok';
    }, error => {
      this.status = 'error';
      this.statusText = error.statusText;
      console.error(error);
    });
  }
}


interface Token {
  access_token: string;
  refresh_token: string;
  id_token: string;
  token_type: string;
  scope: string;
  expires_in: number;
}

interface Request {
  grant_type: string;
  domain: string;
  client_id: string;
  client_secret: string;
  audience: string;
  username: string;
  password: string;
  scope: string;
  code: string;
  redirect_uri: string;
  refresh_token: string;
}
