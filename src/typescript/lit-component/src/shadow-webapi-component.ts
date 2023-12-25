import { LitElement, css, html } from 'lit';
import { customElement, property, query } from 'lit/decorators.js';
import { DialogComponent } from './dialog-component';

@customElement('shadow-webapi-component')
export class ShadowWebApiComponent extends LitElement {
  #value: string;
  #internals: ElementInternals;
  static formAssociated = true;

  constructor() {
    super();
    this.#value = 'init';
    this.#internals = this.attachInternals();
  }

  get value(): string {
    return this.#value;
  }

  set value(newValue: string) {
    this.#value = newValue;
    this.#internals.setFormValue(newValue);
  }

  private _onChange(e: { target: HTMLInputElement }) {
    this.value = (e.target as HTMLInputElement).value;
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

  @query('#dialog')
  private dialog!: DialogComponent;

  private openDialog() {
    this.dialog.open();
  }

  static styles = css`
    :host {
      color: red;
    }
  `;

  render() {
    return html`
      <p>Shadow Hello, ${this.name}!</p>
      <input
        type="text"
        placeholder="name"
        id="name"
        @change=${this._onChange}
        name=${this.inputName}
        .value="${this.#value}"
      />
      <button type="button" @click=${this._onClick}>検索</button>
      <button type="button" @click=${this.openDialog}>ダイアログ</button>
      <dialog-component id="dialog"></dialog-component>
    `;
  }
}
