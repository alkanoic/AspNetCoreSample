import { type Meta, type StoryObj } from "@storybook/vue3";
import NavbarMenu from "../../components/NavbarMenu.vue";

const meta: Meta<typeof NavbarMenu> = {
  component: NavbarMenu,
  render: (args) => ({
    components: { NavbarMenu },
    setup: () => ({ args }),
    template: `
      <NavbarMenu  />
    `,
  }),
};

export default meta;

type Story = StoryObj<typeof NavbarMenu>;

export const Default: Story = {};
