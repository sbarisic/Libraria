using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Libraria.Rendering {
	public class FrameBuffer : OpenGLBuffer<FrameBuffer> {
		public FrameBuffer() {
			//ID = GL.GenFramebuffer();
			GL.CreateFramebuffers(1, out ID);
			//Bind();
		}

		public override FrameBuffer Bind() {
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
			return this;
		}

		public override FrameBuffer Unbind() {
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			return this;
		}

		public void BindRenderBuffer(RenderBuffer RBuffer, FramebufferAttachment Attachment) {
			//GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, Attachment, RenderbufferTarget.Renderbuffer, RBuffer.ID);
			GL.NamedFramebufferRenderbuffer(ID, Attachment, RenderbufferTarget.Renderbuffer, RBuffer.ID);
		}

		public void BindTexture(Texture2D Tex, FramebufferAttachment Attachment) {
			//GL.FramebufferTexture(FramebufferTarget.Framebuffer, Attachment, Tex.ID, 0);
			GL.NamedFramebufferTexture(ID, Attachment, Tex.ID, 0);
		}

		public void DrawBuffer(DrawBufferMode Buffer) {
			//GL.DrawBuffer(Buffer);
			GL.NamedFramebufferDrawBuffer(ID, Buffer);
		}

		public void DrawBuffer(params DrawBuffersEnum[] Buffers) {
			GL.NamedFramebufferDrawBuffers(ID, Buffers.Length, Buffers);
		}

		public override void Destroy() {
			GL.DeleteFramebuffer(ID);
		}
	}
}
