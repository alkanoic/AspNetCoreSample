import { LitElement, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';

@customElement('light-webapi-component')
export class LightWebApiComponent extends LitElement {
  protected createRenderRoot() {
    return this;
  }

  @property()
  name?: string = 'World';

  @property()
  inputName: string = '';

  private async _onClick() {
    if (this.name == null) {
      return;
    }
    const response = await fetch('https://localhost:7035/Simple', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ input: this.name }),
    });
    const result = await response.json();
    console.log(result);
  }

  render() {
    return html`
      <div class="form-group">
        <lable>${this.name}!</lable>
        <input
          type="text"
          class="form-control"
          placeholder="name"
          name=${this.inputName}
          .value="${this.name}"
        />
        <button type="button" class="btn btn-secondary" @click=${this._onClick}>検索</button>
        <button
          type="button"
          class="btn btn-primary"
          data-bs-toggle="modal"
          data-bs-target="#${this.inputName}"
        >
          Open Dialog
        </button>
        <bs-dialog-component dialogId="${this.inputName}"></bs-dialog-component>
      </div>
    `;
  }
}
